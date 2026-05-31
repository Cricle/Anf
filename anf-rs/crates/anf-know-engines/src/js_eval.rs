use boa_engine::{js_string, Context, JsValue, Source};

/// Evaluate a JavaScript expression and return the result as a string.
pub fn eval(js_code: &str) -> anf_core::Result<String> {
    let mut ctx = Context::default();
    let result = ctx
        .eval(Source::from_bytes(js_code))
        .map_err(|e| anf_core::AnfError::JsEval(format!("boa eval error: {e:?}")))?;
    Ok(js_val_to_str(&result, &mut ctx))
}

/// Evaluate JS that returns an array, return each element as a String.
pub fn eval_to_vec(js_code: &str) -> anf_core::Result<Vec<String>> {
    let mut ctx = Context::default();
    let result = ctx
        .eval(Source::from_bytes(js_code))
        .map_err(|e| anf_core::AnfError::JsEval(format!("boa eval error: {e:?}")))?;

    // Register result as global, then stringify it
    ctx.register_global_property(
        js_string!("__result__"),
        result,
        boa_engine::property::Attribute::all(),
    )
    .map_err(|e| anf_core::AnfError::JsEval(format!("register: {e:?}")))?;

    let json_str = ctx
        .eval(Source::from_bytes(
            "try { JSON.stringify(__result__) } catch(e) { '[]' }",
        ))
        .map(|v| js_val_to_str(&v, &mut ctx))
        .unwrap_or_else(|_| "[]".to_string());

    // Parse as JSON array
    if let Ok(arr) = serde_json::from_str::<Vec<serde_json::Value>>(&json_str) {
        Ok(arr
            .iter()
            .map(|v| match v {
                serde_json::Value::String(s) => s.clone(),
                other => other.to_string(),
            })
            .collect())
    } else {
        Ok(vec![json_str])
    }
}

/// Evaluate JS and parse result as JSON value
pub fn eval_to_json(js_code: &str) -> anf_core::Result<serde_json::Value> {
    let mut ctx = Context::default();
    let result = ctx
        .eval(Source::from_bytes(js_code))
        .map_err(|e| anf_core::AnfError::JsEval(format!("boa eval error: {e:?}")))?;

    let json_str = if result.is_string() {
        js_val_to_str(&result, &mut ctx)
    } else {
        ctx.register_global_property(
            js_string!("__result__"),
            result,
            boa_engine::property::Attribute::all(),
        )
        .map_err(|e| anf_core::AnfError::JsEval(format!("register: {e:?}")))?;
        ctx.eval(Source::from_bytes("JSON.stringify(__result__)"))
            .map(|v| js_val_to_str(&v, &mut ctx))
            .unwrap_or_default()
    };
    serde_json::from_str(&json_str)
        .map_err(|e| anf_core::AnfError::JsEval(format!("json parse: {e}")))
}

fn js_val_to_str(val: &JsValue, ctx: &mut Context) -> String {
    match val {
        JsValue::String(s) => s.to_std_string_escaped(),
        JsValue::Null | JsValue::Undefined => String::new(),
        JsValue::Boolean(b) => b.to_string(),
        JsValue::Integer(i) => i.to_string(),
        JsValue::Rational(f) => f.to_string(),
        _ => val
            .to_string(ctx)
            .map(|s| s.to_std_string_escaped())
            .unwrap_or_default(),
    }
}
