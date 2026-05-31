-- Kuaikan Comics (快看漫画) Lua Plugin
-- Provides: search, get_proposal

function engine_name()
    return "Kuaikan"
end

function search(keyword, skip, take)
    local page = 1
    if take > 0 and skip >= take then
        page = math.floor(skip / take) + 1
    end
    local url = "https://www.kuaikanmanhua.com/v1/search/topic?q=" .. keyword .. "&f=" .. page .. "&size=" .. take
    local ok, body = pcall(function()
        return http.get_string(url, {
            referrer = "https://www.kuaikanmanhua.com/",
        })
    end)
    if not ok then
        return { support = true, snapshots = {}, total = 0 }
    end
    local snapshots = {}

    -- Parse JSON manually: look for "hit" array
    -- Use simple pattern matching for the JSON response
    local total = 0
    for total_match in body:gmatch('"total"%s*:%s*(%d+)') do
        total = tonumber(total_match) or 0
        break
    end

    -- Extract items from hit array
    local hit_start = body:find('"hit"%s*:%s*%[')
    if not hit_start then
        return { support = true, snapshots = snapshots, total = 0 }
    end

    -- Find each item block
    local pos = hit_start
    while true do
        local item_start = body:find('{', pos + 1)
        if not item_start then break end

        -- Find matching closing brace
        local depth = 1
        local i = item_start + 1
        while i <= #body and depth > 0 do
            local c = body:sub(i, i)
            if c == '{' then depth = depth + 1
            elseif c == '}' then depth = depth - 1 end
            i = i + 1
        end
        if depth ~= 0 then break end

        local item_str = body:sub(item_start, i - 1)

        -- Extract fields
        local id = item_str:match('"id"%s*:%s*(%d+)')
        local title = item_str:match('"title"%s*:%s*"([^"]*)"')
        local image = item_str:match('"vertical_image_url"%s*:%s*"([^"]*)"')
        local nickname = item_str:match('"nickname"%s*:%s*"([^"]*)"')

        if id and title and title ~= "" then
            table.insert(snapshots, {
                name = title,
                author = nickname or "",
                image_uri = image or "",
                target_url = "https://www.kuaikanmanhua.com/web/topic/" .. id,
                source_name = "Kuaikan",
            })
        end

        pos = i
        if #snapshots >= take then break end
    end

    return { support = true, snapshots = snapshots, total = total }
end

function get_proposal(take)
    local ok, body = pcall(function()
        return http.get_string("https://www.kuaikanmanhua.com/", {
            referrer = "https://www.kuaikanmanhua.com/",
        })
    end)
    if not ok then
        return {}
    end
    local snapshots = {}

    -- Extract topic links
    local links = html.select_all(body, "a[href*='/web/topic/']")
    local titles = html.select_all(body, ".itemTitle")

    for i, link_html in ipairs(links) do
        if #snapshots >= take then break end
        local href = html.attr(link_html, "a", "href") or ""
        local id = href:match("([^/]+)$")
        if id and not seen_id then
            local title_html = titles[i] or ""
            local title = html.text(title_html, "*") or ""
            if title == "" then
                title = title_html:gsub("<[^>]+>", ""):match("^%s*(.-)%s*$") or ""
            end
            title = title:match("^%s*(.-)%s*$") or ""
            if title ~= "" and href ~= "" then
                local full_url = href
                if not href:match("^https?://") then
                    full_url = "https://www.kuaikanmanhua.com" .. href
                end
                table.insert(snapshots, {
                    name = title,
                    author = "",
                    image_uri = "",
                    target_url = full_url,
                    source_name = "Kuaikan",
                })
            end
        end
    end

    return snapshots
end
