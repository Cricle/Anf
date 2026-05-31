-- Tencent Comics (腾讯动漫) Lua Plugin

function engine_name()
    return "Tencent"
end

function search(keyword, skip, take)
    local url = "https://ac.qq.com/Comic/searchList?search=" .. keyword
    local ok, body = pcall(function()
        return http.get_string(url, {
            host = "ac.qq.com",
            referrer = "https://ac.qq.com/",
        })
    end)
    if not ok or #body < 500 then
        return { support = true, snapshots = {}, total = 0 }
    end

    local snapshots = {}
    local seen = {}

    -- Scrape comic links: each comic has two <a> tags (status + title)
    -- We collect by href and take the title text
    local links = html.select_all(body, "a[href*='/Comic/comicInfo/id/']")
    for _, link_html in ipairs(links) do
        if #snapshots >= take then break end
        local href = html.attr(link_html, "a", "href") or ""
        local title = html.text(link_html, "a") or ""
        title = title:match("^%s*(.-)%s*$") or ""

        local id = href:match("id/(%d+)")
        if id and not seen[id] and title ~= "" and not title:match("^更新至") and not title:match("^全%d+话$") then
            seen[id] = true
            table.insert(snapshots, {
                name = title,
                author = "",
                image_uri = "",
                target_url = "https://ac.qq.com/Comic/comicInfo/id/" .. id,
                source_name = "Tencent",
            })
        end
    end

    return { support = true, snapshots = snapshots, total = #snapshots }
end

function get_proposal(take)
    local ok, body = pcall(function()
        return http.get_string("https://ac.qq.com/", {
            host = "ac.qq.com",
            referrer = "https://ac.qq.com/",
        })
    end)
    if not ok then
        return {}
    end

    local snapshots = {}
    local seen = {}

    local links = html.select_all(body, "a[href*='/Comic/comicInfo/id/']")
    for _, link_html in ipairs(links) do
        if #snapshots >= take then break end
        local href = html.attr(link_html, "a", "href") or ""
        local title = html.text(link_html, "a") or ""
        title = title:match("^%s*(.-)%s*$") or ""

        local id = href:match("id/(%d+)")
        if id and not seen[id] and title ~= "" and #title <= 50 and not title:match("^更新至") and not title:match("^全%d+话$") then
            seen[id] = true
            table.insert(snapshots, {
                name = title,
                author = "",
                image_uri = "",
                target_url = "https://ac.qq.com/Comic/comicInfo/id/" .. id,
                source_name = "Tencent",
            })
        end
    end

    return snapshots
end
