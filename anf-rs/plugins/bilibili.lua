-- Bilibili Manga (哔哩哔哩漫画) Lua Plugin

function engine_name()
    return "Bilibili"
end

function search(keyword, skip, take)
    -- Search API returns error 99, scrape SSR page instead
    local url = "https://manga.bilibili.com/search?keyword=" .. keyword
    local ok, body = pcall(function()
        return http.get_string(url, {
            referrer = "https://manga.bilibili.com/",
        })
    end)
    if not ok then
        return { support = true, snapshots = {}, total = 0 }
    end

    local snapshots = {}
    local seen = {}

    -- SSR renders results client-side, but we can try to find data in script tags
    -- Look for comic data in JSON embedded in page
    for id, title in body:gmatch('"manga_id"%s*:?%s*"?(%d+)"?.-"title"%s*:?%s*"([^"]+)"') do
        if #snapshots >= take then break end
        if not seen[id] and title ~= "" then
            seen[id] = true
            table.insert(snapshots, {
                name = title,
                author = "",
                image_uri = "",
                target_url = "https://manga.bilibili.com/detail/mc" .. id,
                source_name = "Bilibili",
            })
        end
    end

    -- Fallback: try rendered HTML patterns
    if #snapshots == 0 then
        for id, title in body:gmatch('/detail/mc(%d+).-title="([^"]+)"') do
            if #snapshots >= take then break end
            if not seen[id] and title ~= "" then
                seen[id] = true
                table.insert(snapshots, {
                    name = title,
                    author = "",
                    image_uri = "",
                    target_url = "https://manga.bilibili.com/detail/mc" .. id,
                    source_name = "Bilibili",
                })
            end
        end
    end

    return { support = true, snapshots = snapshots, total = #snapshots }
end

function get_proposal(take)
    local ok, body = pcall(function()
        return http.get_string("https://manga.bilibili.com/", {
            referrer = "https://manga.bilibili.com/",
        })
    end)
    if not ok then
        return {}
    end

    local snapshots = {}
    local seen = {}

    -- The SSR page has JSON data in a script tag with comic info
    -- Extract from banner data: "comic_title":"黎明之剑" ... "jumpUrl":"/detail/mc29121"
    for title, id in body:gmatch('"comic_title"%s*:%s*"([^"]+)".-/detail/mc(%d+)') do
        if #snapshots >= take then break end
        if not seen[id] and title ~= "" then
            seen[id] = true
            table.insert(snapshots, {
                name = title,
                author = "",
                image_uri = "",
                target_url = "https://manga.bilibili.com/detail/mc" .. id,
                source_name = "Bilibili",
            })
        end
    end

    -- Also try card_title pattern
    for title, id in body:gmatch('"card_title"%s*:%s*"([^"]+)".-/detail/mc(%d+)') do
        if #snapshots >= take then break end
        if not seen[id] and title ~= "" then
            seen[id] = true
            table.insert(snapshots, {
                name = title,
                author = "",
                image_uri = "",
                target_url = "https://manga.bilibili.com/detail/mc" .. id,
                source_name = "Bilibili",
            })
        end
    end

    -- Fallback: match href patterns with nearby title attributes
    if #snapshots == 0 then
        for id in body:gmatch('/detail/mc(%d+)%?from=manga_homepage') do
            if #snapshots >= take then break end
            if not seen[id] then
                local title = body:match('mc' .. id .. '.-"title"%s*=%s*"([^"]+)"') or ""
                if title ~= "" then
                    seen[id] = true
                    table.insert(snapshots, {
                        name = title,
                        author = "",
                        image_uri = "",
                        target_url = "https://manga.bilibili.com/detail/mc" .. id,
                        source_name = "Bilibili",
                    })
                end
            end
        end
    end

    return snapshots
end
