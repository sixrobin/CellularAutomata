namespace CellularAutomata
{
    using UnityEngine;

    public static class RulesParser
    {
        /// <summary>
        /// Parses the birth and survive rules of a cellular automaton.
        /// The rule uses the format B[..]/S[..] where [..] is the valid neighbours counts.
        /// Each neighbours count is split by a comma (,), and chains can be marked using a hyphen (-).
        /// For instance, B1-3/S7,8 means cells are born if there are 1, 2 or 3 neighbours, and survive if there are 7 or 8 neighbours.
        /// Returns a Vector2Int. Each birth neighbours count is converted using 2^n formula and added to the x value. Same for survive neighbours count with y value.
        /// To check a cell, simply count its neighbours, convert it using the same 2^n formula and compare it to the rule using a AND operator.
        /// </summary>
        /// <param name="rules">Ruleset string (B[..]/S[..] where [..] is the valid neighbours counts).</param>
        /// <returns>Rules as a Vector2Int format.</returns>
        public static Vector2Int ParseRuleset(string rules)
        {
            try
            {
                rules = rules.ToUpper();

                // Split birth and survive rules.
                string[] rulesSplit = rules.Split('/');
                string birthRules = rulesSplit[0];
                string surviveRules = rulesSplit[1];
                // TODO: Handle neighbourhood letter (M=Moore, N=Neumann).

                // Convert both rules.
                return new Vector2Int(ParseNeighboursRule(birthRules), ParseNeighboursRule(surviveRules));
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Could not parse rules {rules}! Make sure rules format is B[x]/S[x].\nException message: {e}");
                return Vector2Int.zero;
            }
        }

        private static int ParseNeighboursRule(string rule)
        {
            int ruleToInt = 0;
            rule = rule.Remove(0, 1); // Remove letter prefix.

            if (rule.Length == 0)
                return 0;
            
            string[] ruleChains = rule.Split(',');

            foreach (string ruleChain in ruleChains)
            {
                if (ruleChain.Contains('-'))
                {
                    string[] minMaxValues = ruleChain.Split('-');
                    int minValue = int.Parse(minMaxValues[0]);
                    int maxValue = int.Parse(minMaxValues[1]);

                    for (int i = minValue; i <= maxValue; ++i)
                    {
                        int neighboursCount = (int)Mathf.Pow(2, i); // Convert value to 2^value.
                        ruleToInt += neighboursCount; // Add value to birth rule.
                    }
                }
                else
                {
                    int neighboursCount = int.Parse(ruleChain); // Parse value.
                    neighboursCount = (int)Mathf.Pow(2, neighboursCount); // Convert value to 2^value.
                    ruleToInt += neighboursCount; // Add value to birth rule.
                }
            }

            return ruleToInt;
        }
    }
}