local x = false
local v = 0.0

function DrawUI()
    imgui.Begin("External Lua UI")

    -- A standard slider
    settings.myValue = imgui.SliderFloat("My Value", settings.myValue, settings.minLimit, settings.maxLimit)

    -- The limits are just variables! Change them from C# or even from another Lua control.
    settings.minLimit = imgui.SliderFloat("Min Limit", settings.minLimit, -100, 100)
    settings.maxLimit = imgui.SliderFloat("Max Limit", settings.maxLimit, -100, 100)

    if (imgui.Button("Test")) then
        x = not x;
    end

    if (x) then
        imgui.SliderFloat("Sex", v, 0, 5)
    end

    imgui.End()
end

return DrawUI
