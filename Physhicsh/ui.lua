function DrawUI()
    imgui.Begin("External Lua UI")

    -- A standard slider
    settings.myValue = imgui.SliderFloat("My Value", settings.myValue, settings.minLimit, settings.maxLimit)

    -- The limits are just variables! Change them from C# or even from another Lua control.
    settings.minLimit = imgui.SliderFloat("Min Limit", settings.minLimit, -100, 100)
    settings.maxLimit = imgui.SliderFloat("Max Limit", settings.maxLimit, -100, 100)

    imgui.End()
end

return DrawUI
