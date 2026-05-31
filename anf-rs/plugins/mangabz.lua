-- Mangabz (漫画DB) Lua Plugin
-- Also used by Xmanhua via BASE_URL override

function engine_name()
    return "Mangabz"
end

BASE_URL = BASE_URL or "https://www.mangabz.com"

function search(keyword, skip, take)
    local page = 1
    if take > 0 and skip >= take then
        page = math.floor(skip / take) + 1
    end
    local url = BASE_URL .. "/search/?title=" .. keyword .. "&page=" .. page
    local ok, body = pcall(function()
        return http.get_string(url, {
            referrer = BASE_URL .. "/",
            headers = {
                ["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4437.0 Safari/537.36 Edg/91.0.831.1"
            }
        })
    end)
    if not ok then
        return { support = true, snapshots = {}, total = 0 }
    end

    local snapshots = {}
    local items = html.select_all(body, "div.mh-item")

    for i, item_html in ipairs(items) do
        if #snapshots >= take then break end

        local a_html = html.select_one(item_html, "div.mh-item-detali h2.title a")
        local title = ""
        local href = ""
        if a_html then
            title = html.text(a_html, "a") or ""
            href = html.attr(a_html, "a", "href") or ""
        end

        local cover = ""
        local p_html = html.select_one(item_html, "p")
        if p_html then
            local style = html.attr(p_html, "p", "style") or ""
            cover = style:match("url%((.-)%)") or ""
        end

        local auth = ""
        local auth_a = html.select_one(item_html, "p.author span a")
        if auth_a then
            auth = html.text(auth_a, "a") or ""
        end

        if title ~= "" and href ~= "" then
            local full_url = href
            if not href:match("^https?://") then
                full_url = BASE_URL .. href
            end
            table.insert(snapshots, {
                name = title,
                author = auth,
                image_uri = cover,
                target_url = full_url,
                source_name = engine_name(),
            })
        end
    end

    return { support = true, snapshots = snapshots, total = #snapshots }
end

function get_proposal(take)
    local ok, body = pcall(function()
        return http.get_string(BASE_URL .. "/manga-list/", {
            referrer = BASE_URL .. "/",
            headers = {
                ["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4437.0 Safari/537.36 Edg/91.0.831.1"
            }
        })
    end)
    if not ok then
        return {}
    end

    local snapshots = {}
    local items = html.select_all(body, "div.mh-item")

    for _, item_html in ipairs(items) do
        if #snapshots >= take then break end

        local a_html = html.select_one(item_html, "div.mh-item-detali h2.title a")
        local title = ""
        local href = ""
        if a_html then
            title = html.text(a_html, "a") or ""
            href = html.attr(a_html, "a", "href") or ""
        end

        local cover = ""
        local p_html = html.select_one(item_html, "p")
        if p_html then
            local style = html.attr(p_html, "p", "style") or ""
            cover = style:match("url%((.-)%)") or ""
        end

        local auth = ""
        local auth_a = html.select_one(item_html, "p.author span a")
        if auth_a then
            auth = html.text(auth_a, "a") or ""
        end

        if title ~= "" and href ~= "" then
            local full_url = href
            if not href:match("^https?://") then
                full_url = BASE_URL .. href
            end
            table.insert(snapshots, {
                name = title,
                author = auth,
                image_uri = cover,
                target_url = full_url,
                source_name = engine_name(),
            })
        end
    end

    return snapshots
end
