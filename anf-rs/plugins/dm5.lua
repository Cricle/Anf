-- Dm5 (动漫屋) Lua Plugin
-- Also used by Jisu (极速漫画) via base_url parameter

function engine_name()
    return "Dm5"
end

BASE_URL = BASE_URL or "http://www.dm5.com"

function search(keyword, skip, take)
    local page = 1
    if take > 0 and skip >= take then
        page = math.floor(skip / take)
    end
    local url = BASE_URL .. "/search?title=" .. keyword .. "&language=1"
    local host = BASE_URL:match("https?://([^/]+)") or ""
    local ok, body = pcall(function()
        return http.get_string(url, {
            host = host,
            referrer = BASE_URL .. "/",
        })
    end)
    if not ok then
        return { support = true, snapshots = {}, total = 0 }
    end

    local snapshots = {}
    local items = html.select_all(body, "div.box-body ul li div.mh-item")

    for i, item_html in ipairs(items) do
        if #snapshots >= take then break end

        local a_html = html.select_one(item_html, "div.mh-item-detali h2.title a")
        local title = ""
        local href = ""
        if a_html then
            title = html.text(a_html, "a") or ""
            href = html.attr(a_html, "a", "href") or ""
        end

        local auth = ""
        local auth_a = html.select_one(item_html, "p.author span a")
        if auth_a then
            auth = html.text(auth_a, "a") or ""
        end

        local cover = ""
        local p_html = html.select_one(item_html, "p")
        if p_html then
            local style = html.attr(p_html, "p", "style") or ""
            cover = style:match("url%((.-)%)") or ""
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
                source_name = "Dm5",
            })
        end
    end

    return { support = true, snapshots = snapshots, total = #snapshots }
end

function get_proposal(take)
    local host = BASE_URL:match("https?://([^/]+)") or ""
    local ok, body = pcall(function()
        return http.get_string(BASE_URL .. "/", {
            host = host,
            referrer = BASE_URL .. "/",
        })
    end)
    if not ok then
        return {}
    end

    local snapshots = {}
    for i = 1, 6 do
        if #snapshots >= take then break end
        local section = html.select_all(body, "div#index-update-" .. i .. " div ul li div div.mh-tip-wrap div")
        for _, root_html in ipairs(section) do
            if #snapshots >= take then break end
            local href = html.attr(root_html, "a", "href") or ""
            local title_a = html.select_one(root_html, "div.mh-item-tip-detali h2 a")
            local title = ""
            if title_a then
                title = html.attr(title_a, "a", "title") or html.text(title_a, "a") or ""
            end

            local auth = ""
            local auth_a = html.select_one(root_html, "div.mh-item-tip-detali p.author span a")
            if auth_a then
                auth = html.text(auth_a, "a") or ""
            end

            if href ~= "" then
                local full_url = href
                if not href:match("^https?://") then
                    full_url = BASE_URL .. href
                end
                table.insert(snapshots, {
                    name = title,
                    author = auth,
                    image_uri = "",
                    target_url = full_url,
                    source_name = "Dm5",
                })
            end
        end
    end

    return snapshots
end
