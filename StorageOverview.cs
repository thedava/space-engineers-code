class StorageOverview {
    const int PANEL_LINES = 18;
    string[] panels = new string[] {
        "Ore Counter LCD #1",
        "Ore Counter LCD #2",
        "Ore Counter LCD #3"
    };

    string[] storageObjects = new string[] {
        "Refinery",
        "Cargo",
        "Arc",
        "Assembler",
        "Connector"
    };

    void Main()
    {
        var consolidated = new Dictionary<String, float>();
        List<String> list = new List<String>();
        
        // Clear displays
        for (int p = 0; p < panels.Length; p++) {
            IMyTextPanel panel = getPanel(panels[p]);
            panel.WritePublicText("", false);
            panel.ShowTextureOnScreen();
            panel.ShowPublicTextOnScreen();
        }

        // Append all types of storage objects
        foreach (var storageObject in storageObjects)
            appendConsolidated(consolidated, storageObject);

        var enumerator = consolidated.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var pair = enumerator.Current;
            string itemKey = pair.Key;
            float itemValue = pair.Value;

            string txt = itemKey.Split('|')[0] + "  -  ";
            string amt = amountFormatter(itemValue,itemKey.Split('|')[1]);
            txt += amt;
            list.Add(txt);
        }
        list.Sort();

        var datePrinted = false;
        for (int p = 0; p < panels.Length; p++) {
            IMyTextPanel panel = getPanel(panels[p]);
            var remainingLines = PANEL_LINES;
            var isLastDisplay = p == (panels.Length - 1);

            if (isLastDisplay)
                remainingLines--;

            var text = "";
            var removeCount = 0;
            
            for (int i = 0; i < Math.Min(list.Count, remainingLines); i++) {
                removeCount++;
                text += list[i] + "\n";
            }
            for (; removeCount > 0; removeCount--)
                list.RemoveAt(0);

            if (isLastDisplay && list.Count > 0)
                text += $"Remaining Items: {list.Count}\n";
            
            if (!datePrinted && list.Count == 0) {
                datePrinted = true;
                text += "Last Updated: " + DateTime.Now.ToString() + "\n";
            }

            Echo($"Output Panel #{p}");
            Echo(text);
            Echo("================");

            panel.WritePublicText(text, false);
            panel.ShowTextureOnScreen();
            panel.ShowPublicTextOnScreen();
        }

        (GridTerminalSystem.GetBlockWithName("Ore Counter Code Timer") as IMyTimerBlock).ApplyAction("Start");
    }

    void appendConsolidated(Dictionary<string, float> consolidated, string objectName) {
        var containerList = new List<IMyTerminalBlock>();
        GridTerminalSystem.SearchBlocksOfName(objectName, containerList);

        for (int i = 0; i < containerList.Count; i++)
        {
             if (containerList[i] is IMyCargoContainer) {
                var containerInvOwner = containerList[i] as IMyInventoryOwner;
                var containerItems = containerInvOwner.GetInventory(0).GetItems();
                for(int j = containerItems.Count - 1; j >= 0; j--)
                {
                    var itemName = decodeItemName(containerItems[j].Content.SubtypeName,
                                      containerItems[j].Content.TypeId.ToString()) + "|" +
                                      containerItems[j].Content.TypeId.ToString();
                    var amount = (float)containerItems[j].Amount;
                    if (!consolidated.ContainsKey(itemName)) {
                       consolidated.Add(itemName, amount);
                    } else {
                       consolidated[itemName] += amount;
                    }
                }
            }
        }
    }

    string amountFormatter(float amt, string typeId) {
        if (typeId.EndsWith("_Ore") || typeId.EndsWith("_Ingot")) {
            if (amt > 1000.0f) {
              return "" + Math.Round((float)amt / 1000, 2).ToString("###,###,##0.00") + "K";
            } else {
              return "" + Math.Round((float)amt, 2).ToString("###,###,##0.00");
            }
        }
        return "" + Math.Round((float)amt, 0).ToString("###,###,##0");
    }

    IMyTextPanel getPanel(string panelName) {
        try {
            return (IMyTextPanel)GridTerminalSystem.GetBlockWithName(panelName);
        } catch (Exception error) {
            throw new Exception($"Unable to find panel with name '{panelName}'", error);
        }
    }

    string decodeItemName(string name, string typeId) {
        if (typeId.EndsWith("_Ore")) {
            return (name.Equals("Stone")) ? name : name + " Ore";
        }
        else if (typeId.EndsWith("_Ingot")) {
            if (name.Equals("Stone"))
                return "Gravel";

            if (name.Equals("Magnesium"))
                return name + " Powder";

            if (name.Equals("Silicon"))
                return name + " Wafer";

           return name + " Ingot";
        }
        else if (name.StartsWith("NATO_")) {
            return "NATO Ammo";
        }

        switch (name) {
            case "Construction":                 return "Construction Component";
            case "MetalGrid":                    return "Metal Grid";
            case "InteriorPlate":                return "Interior Plate";
            case "SteelPlate":                   return "Steel Plate";
            case "SmallTube":                    return "Small Steel Tube";
            case "LargeTube":                    return "Large Steel Tube";
            case "BulletproofGlass":             return "Bulletproof Glass";
            case "Reactor":                      return "Reactor Component";
            case "Thrust":                       return "Thruster Component";
            case "GravityGenerator":             return "GravGen Component";
            case "Medical":                      return "Medical Component";
            case "RadioCommunication":           return "Radio Component";
            case "Detector":                     return "Detector Component";
            case "SolarCell":                    return "Solar Cell";
            case "PowerCell":                    return "Power Cell";
            case "AutomaticRifleItem":           return "Rifle";
            case "AutomaticRocketLauncher":      return "Rocket Launcher";
            case "WelderItem":                   return "Welder";
            case "AngleGrinderItem":             return "Grinder";
            case "HandDrillItem":                return "Hand Drill";
            case "Missile200mm":                 return "Missile";

            default: return name;
        }
    }
}
