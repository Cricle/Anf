-- =============================================================
-- Example Anf Lua Plugin
-- =============================================================
-- Each plugin must define these globals:
--   engine_name()              -> string
--   address()                  -> string  (base URL of the site)
--   condition(url, host)       -> bool    (should this plugin handle this URL?)
--   get_chapters(url)          -> table   (comic entity with chapters)
--   get_pages(url)             -> table   (list of pages)
--   get_image(url)             -> string  (raw image bytes)
--
-- Optional globals:
--   favicon_address()          -> string
--   order()                    -> number  (higher = checked first)
--
-- Available globals injected by Rust:
--   http.get_string(url [, opts]) -> string
--   http.get(url [, opts])        -> bytes (as LuaString)
--   http.post(url, body [, opts]) -> string
--   html.parse(str)               -> table
--   html.text(html_str, selector) -> string|nil
--   html.attr(html_str, sel, a)   -> string|nil
--   html.select_one(html_str, sel)-> string|nil
--   html.select_all(html_str, sel)-> {string, ...}
-- =============================================================

function engine_name()
    return "Example"
end

function address()
    return "https://example.com"
end

function favicon_address()
    return "https://example.com/favicon.ico"
end

function order()
    return 0
end

function condition(url, host)
    return host == "example.com"
end

function get_chapters(url)
    -- Fetch the page
    local body = http.get_string(url, {
        referrer = "https://example.com/",
        headers = { ["User-Agent"] = "Mozilla/5.0" }
    })

    -- Parse with built-in html helpers
    local title = html.text(body, "h1.title") or "Unknown"
    local desc  = html.text(body, "div.description") or ""
    local image = html.attr(body, "div.cover img", "src") or ""

    -- Extract chapter links
    local links = html.select_all(body, "ul.chapters li a")
    local chapters = {}
    for i, link_html in ipairs(links) do
        local href  = html.attr(link_html, "a", "href") or ""
        local label = html.text(link_html, "a") or ("Chapter " .. i)
        table.insert(chapters, {
            target_url = "https://example.com" .. href,
            title = label,
        })
    end

    return {
        name = title,
        descript = desc,
        image_url = image,
        chapters = chapters,
    }
end

function get_pages(url)
    local body = http.get_string(url, {
        referrer = "https://example.com/",
    })

    local imgs = html.select_all(body, "div.page img")
    local pages = {}
    for i, img_html in ipairs(imgs) do
        local src = html.attr(img_html, "img", "src") or ""
        table.insert(pages, {
            name = tostring(i),
            target_url = src,
        })
    end
    return pages
end

function get_image(url)
    return http.get(url, {
        referrer = "https://example.com/",
        headers = { ["User-Agent"] = "Mozilla/5.0" }
    })
end
