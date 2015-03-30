﻿using System;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX.Direct3D9;
using Font = SharpDX.Direct3D9.Font;

namespace LeBlanc.Helper
{
    internal class AssassinManager
    {
        public static Font Text, TextBold;

        public static void Init()
        {
            Load();
        }

        private static void Load()
        {
            TextBold = new Font(
                Drawing.Direct3DDevice,
                new FontDescription
                {
                    FaceName = "Tahoma",
                    Height = 13,
                    Weight = FontWeight.Bold,
                    OutputPrecision = FontPrecision.Default,
                    Quality = FontQuality.ClearType,

                });

            Text = new Font(
                Drawing.Direct3DDevice,
                new FontDescription
                {
                    FaceName = "Tahoma",
                    Height = 13,
                    OutputPrecision = FontPrecision.Default,
                    Quality = FontQuality.ClearType,

                });

            Config.LeBlanc.AddSubMenu(new Menu("Set Priority Targets", "MenuAssassin"));
            Config.LeBlanc.SubMenu("MenuAssassin").AddItem(new MenuItem("AssassinActive", "Active").SetValue(true));
            Config.LeBlanc.SubMenu("MenuAssassin")
                .AddItem(new MenuItem("AssassinSearchRange", "    Search Range"))
                .SetValue(new Slider(1400, 2000));
            
            Config.LeBlanc.SubMenu("MenuAssassin")
                .AddItem(
                    new MenuItem("AssassinSelectOption", "    Set:").SetValue(
                        new StringList(new[] { "Single Select", "Multi Select" })));

            Config.LeBlanc.SubMenu("MenuAssassin").AddItem(new MenuItem("xM1", "Enemies:"));
            foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(enemy => enemy.Team != ObjectManager.Player.Team))
            {
                Config.LeBlanc.SubMenu("MenuAssassin")

                    .AddItem(
                        new MenuItem("Assassin" + enemy.ChampionName, "    " + enemy.ChampionName).SetValue(
                            TargetSelector.GetPriority(enemy) > 3));
            }
            Config.LeBlanc.SubMenu("MenuAssassin").AddItem(new MenuItem("xM2", "Other Settings:"));
            
            Config.LeBlanc.SubMenu("MenuAssassin")
                .AddItem(new MenuItem("AssassinSetClick", "    Add/Remove with click").SetValue(true));
            Config.LeBlanc.SubMenu("MenuAssassin")
                .AddItem(
                    new MenuItem("AssassinReset", "    Reset List").SetValue(new KeyBind("T".ToCharArray()[0],
                        KeyBindType.Press)));

            Config.LeBlanc.SubMenu("MenuAssassin").AddSubMenu(new Menu("Drawings", "Draw"));

            Config.LeBlanc.SubMenu("MenuAssassin")
                .SubMenu("Draw")
                .AddItem(new MenuItem("DrawSearch", "Search Range").SetValue(new Circle(true, Color.GreenYellow)));
            Config.LeBlanc.SubMenu("MenuAssassin")
                .SubMenu("Draw")
                .AddItem(new MenuItem("DrawActive", "Active Enemy").SetValue(new Circle(true, Color.GreenYellow)));
            Config.LeBlanc.SubMenu("MenuAssassin")
                .SubMenu("Draw")
                .AddItem(new MenuItem("DrawNearest", "Nearest Enemy").SetValue(new Circle(true, Color.DarkSeaGreen)));
            Config.LeBlanc.SubMenu("MenuAssassin")
                .SubMenu("Draw")
                .AddItem(new MenuItem("DrawStatus", "Show status on the screen").SetValue(true));

            Game.OnUpdate += OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnWndProc += Game_OnWndProc;
        }

        static void ClearAssassinList()
        {
            foreach (
                var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(enemy => enemy.Team != ObjectManager.Player.Team))
            {
                Config.LeBlanc.Item("Assassin" + enemy.ChampionName).SetValue(false);
            }
        }
        private static void OnGameUpdate(EventArgs args)
        {
        }

        public static void DrawText(Font vFont, String vText, float vPosX, float vPosY, SharpDX.ColorBGRA vColor)
        {
            vFont.DrawText(null, vText, (int)vPosX, (int)vPosY, vColor);
        }
        public static void DrawTextBold(Font vFont, String vText, float vPosX, float vPosY, SharpDX.ColorBGRA vColor)
        {
            vFont.DrawText(null, vText, (int)vPosX, (int)vPosY, vColor);
        }


        private static void Game_OnWndProc(WndEventArgs args)
        {

            if (Config.LeBlanc.Item("AssassinReset").GetValue<KeyBind>().Active && args.Msg == 257)
            {
                ClearAssassinList();
                Game.PrintChat(
                    "<font color='#FFFFFF'>Reset Assassin List is Complete! Click on the enemy for Add/Remove.</font>");
            }

            if (args.Msg != (uint)WindowsMessages.WM_LBUTTONDOWN)
            {
                return;
            }

            if (Config.LeBlanc.Item("AssassinSetClick").GetValue<bool>())
            {
                foreach (var objAiHero in from hero in ObjectManager.Get<Obj_AI_Hero>()
                                          where hero.IsValidTarget()
                                          select hero
                                              into h
                                              orderby h.Distance(Game.CursorPos) descending
                                              select h
                                                  into enemy
                                                  where enemy.Distance(Game.CursorPos) < 150f
                                                  select enemy)
                {
                    if (objAiHero != null && objAiHero.IsVisible && !objAiHero.IsDead)
                    {
                        var xSelect =
                            Config.LeBlanc.Item("AssassinSelectOption").GetValue<StringList>().SelectedIndex;

                        switch (xSelect)
                        {
                            case 0:
                                ClearAssassinList();
                                Config.LeBlanc.Item("Assassin" + objAiHero.ChampionName).SetValue(true);
                                Game.PrintChat(
                                    string.Format(
                                        "<font color='FFFFFF'>Added to Assassin List</font> <font color='#09F000'>{0} ({1})</font>",
                                        objAiHero.Name, objAiHero.ChampionName));
                                break;
                            case 1:
                                var menuStatus =
                                    Config.LeBlanc.Item("Assassin" + objAiHero.ChampionName)
                                        .GetValue<bool>();
                                Config.LeBlanc.Item("Assassin" + objAiHero.ChampionName)
                                    .SetValue(!menuStatus);
                                Game.PrintChat(
                                    string.Format("<font color='{0}'>{1}</font> <font color='#09F000'>{2} ({3})</font>",
                                        !menuStatus ? "#FFFFFF" : "#FF8877",
                                        !menuStatus ? "Added to Assassin List:" : "Removed from Assassin List:",
                                        objAiHero.Name, objAiHero.ChampionName));
                                break;
                        }
                    }
                }
            }
        }
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!Config.LeBlanc.Item("AssassinActive").GetValue<bool>())
                return;

            if (Config.LeBlanc.Item("DrawStatus").GetValue<bool>())
            {
                var enemies = ObjectManager.Get<Obj_AI_Hero>().Where(xEnemy => xEnemy.IsEnemy);
                var objAiHeroes = enemies as Obj_AI_Hero[] ?? enemies.ToArray();

                DrawText(TextBold, "Target Mode:", (int)Drawing.Width * 0.89f, (int)Drawing.Height * 0.55f, SharpDX.Color.White); 
                var xSelect = Config.LeBlanc.Item("AssassinSelectOption").GetValue<StringList>().SelectedIndex;
                DrawText(
                    Text, xSelect == 0 ? "Single Target" : "Multi Targets", (int) Drawing.Width * 0.94f,
                    Drawing.Height * 0.55f, SharpDX.Color.White);

                DrawText(TextBold, "Priority Targets", (int)Drawing.Width * 0.89f, (int)Drawing.Height * 0.58f, SharpDX.Color.White);
                DrawText(TextBold, "_____________", (int)Drawing.Width * 0.89f, (int)Drawing.Height * 0.58f, SharpDX.Color.White);

                for (int i = 0; i < objAiHeroes.Count(); i++)
                {
                    var xValue = Config.LeBlanc.Item("Assassin" + objAiHeroes[i].ChampionName).GetValue<bool>();
                    DrawTextBold(
                        xValue ? TextBold : Text, objAiHeroes[i].ChampionName, Drawing.Width * 0.895f,
                        Drawing.Height * 0.58f + (float) (i + 1) * 15,
                        xValue ? SharpDX.Color.GreenYellow : SharpDX.Color.DarkGray);
                }
            }

            var drawSearch = Config.LeBlanc.Item("DrawSearch").GetValue<Circle>();
            var drawActive = Config.LeBlanc.Item("DrawActive").GetValue<Circle>();
            var drawNearest = Config.LeBlanc.Item("DrawNearest").GetValue<Circle>();

            var drawSearchRange = Config.LeBlanc.Item("AssassinSearchRange").GetValue<Slider>().Value;
            if (drawSearch.Active)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, drawSearchRange, drawSearch.Color, 1);
            }

            foreach (
                var enemy in
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(enemy => enemy.Team != ObjectManager.Player.Team)
                        .Where(
                            enemy =>
                                enemy.IsVisible &&
                                Config.LeBlanc.Item("Assassin" + enemy.ChampionName) != null &&
                                !enemy.IsDead)
                        .Where(
                            enemy => Config.LeBlanc.Item("Assassin" + enemy.ChampionName).GetValue<bool>()))
            {
                if (ObjectManager.Player.Distance(enemy) < drawSearchRange)
                {
                    if (drawActive.Active)
                        Render.Circle.DrawCircle(enemy.Position, 115f, drawActive.Color, 1);
                }
                else if (ObjectManager.Player.Distance(enemy) > drawSearchRange &&
                         ObjectManager.Player.Distance(enemy) < drawSearchRange + 400)
                {
                    if (drawNearest.Active)
                        Render.Circle.DrawCircle(enemy.Position, 115f, drawNearest.Color, 1);
                }
            }
        }
    }
}
