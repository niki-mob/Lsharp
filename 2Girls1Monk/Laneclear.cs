/*
 

                                                    __                        .-'''-.                             
       .-''-.                    .---.         ...-'  |`.                    '   _    \                           
     .' .-.  )         .--.      |   |         |      |  |  __  __   ___   /   /` '.   \   _..._       .          
    / .'  / /    .--./)|__|      |   |         ....   |  | |  |/  `.'   `..   |     \  ' .'     '.   .'|          
   (_/   / /    /.''\\ .--.-,.--.|   |           -|   |  | |   .-.  .-.   |   '      |  .   .-.   ..'  |          
        / /    | |  | ||  |  .-. |   |            |   |  | |  |  |  |  |  \    \     / /|  '   '  <    |          
       / /      \`-' / |  | |  | |   |      _  ...'   `--' |  |  |  |  |  |`.   ` ..' / |  |   |  ||   | ____     
      . '       /("'`  |  | |  | |   |    .' | |         |`|  |  |  |  |  |   '-...-'`  |  |   |  ||   | \ .'     
     / /    _.-'\ '---.|  | |  '-|   |   .   | ` --------\ |  |  |  |  |  |             |  |   |  ||   |/  .      
   .' '  _.'.-'' /'""'.|__| |    |   | .'.'| |//`---------'|__|  |__|  |__|             |  |   |  ||    /\  \     
  /  /.-'_.'    ||     || | |    '---.'.'.-'  /                                         |  |   |  ||   |  \  \    
 /    _.'       \'. __//  |_|        .'   \_.'                                          |  |   |  |'    \  \  \   
( _.-'           `'---'                                                                 '--'   '--'------'  '---' 

*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace two_girls_one_monk
{
    class Laneclear : LeeSin
    {
        public static void Clear()
        {
            var minion = MinionManager.GetMinions(Player.ServerPosition, Q.Range).FirstOrDefault();
            if (minion == null || minion.Name.ToLower().Contains("ward")) return;
            if (LeeSinSharp.Config.Item("UseQClear").GetValue<bool>() && Q.IsReady())
            {
                if (Q.Instance.Name == "BlindMonkQOne")
                {
                    Q.Cast(minion, true);
                }
                else if ((minion.HasBuff("BlindMonkQOne", true) ||
                         minion.HasBuff("blindmonkqonechaos", true)) && (Q.IsKillable(minion, 1)) ||
                         Player.Distance(minion) > 500) Q.Cast();
            }
            if (LeeSinSharp.Config.Item("UseWClear").GetValue<bool>() && W.IsReady())
            {
                if ((W.Instance.Name == "BlindMonkWOne") && (Player.HealthPercent <= 80))
                {
                    W.Cast(Player, true);
                }
                else if ((Player.HasBuff("BlindMonkWOne", true) && (Player.HealthPercent <= 75)))
                   W.Cast();
            }
            if (LeeSinSharp.Config.Item("UseEClear").GetValue<bool>() && E.IsReady())
            {
                if (E.Instance.Name == "BlindMonkEOne" && minion.IsValidTarget(E.Range))
                {
                    E.Cast();
                }
                else if (minion.HasBuff("BlindMonkEOne", true) && (Player.Distance(minion) > 450) && HasEnergyFor(false, true, true, false))
                {
                    E.Cast();
                }
            }
        }
            public static void Jgclear()
            {
            var jngm = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();
            if (jngm == null || jngm.Name.ToLower().Contains("ward")) return;
            if (LeeSinSharp.Config.Item("UseQClear").GetValue<bool>() && Q.IsReady())
            {
                if (Q.Instance.Name == "BlindMonkQOne")
                {
                    Q.Cast(jngm, true);
                }
                else if ((jngm.HasBuff("BlindMonkQOne", true) ||
                         jngm.HasBuff("blindmonkqonechaos", true)) && (Q.IsKillable(jngm, 1)) ||
                         Player.Distance(jngm) > 500) Q.Cast();
            }
            if (LeeSinSharp.Config.Item("UseWClear").GetValue<bool>() && W.IsReady())
            {
                if ((W.Instance.Name == "BlindMonkWOne") && (Player.HealthPercent <= 80))
                {
                    W.Cast(Player, true);
                }
                else if ((Player.HasBuff("BlindMonkWOne", true) && (Player.HealthPercent <= 75)))
                    W.Cast();
            }
            if (LeeSinSharp.Config.Item("UseEClear").GetValue<bool>() && E.IsReady())
            {
                if (E.Instance.Name == "BlindMonkEOne" && jngm.IsValidTarget(E.Range))
                {
                    E.Cast();
                }
                else if (jngm.HasBuff("BlindMonkEOne", true) && (Player.Distance(jngm) > 450) && HasEnergyFor(false, true, true, false))
                {
                    E.Cast();
                }
            }
        }
            static bool HasEnergyFor(bool Q, bool W, bool E, bool R)
            {
                float totalCost = 0;

                if (Q)
                    totalCost += Player.Spellbook.GetSpell(SpellSlot.Q).ManaCost;
                if (W)
                    totalCost += Player.Spellbook.GetSpell(SpellSlot.W).ManaCost;
                if (E)
                    totalCost += Player.Spellbook.GetSpell(SpellSlot.E).ManaCost;
                if (R)
                    totalCost += Player.Spellbook.GetSpell(SpellSlot.R).ManaCost;

                if (Player.Mana >= totalCost)
                    return true;
                else
                    return false;
            }  
        }
    }
