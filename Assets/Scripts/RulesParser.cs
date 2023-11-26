namespace CellularAutomata
{
    using UnityEngine;

    public static class RulesParser
    {
        // TODO: Rewrite this summary with new ruleset format.
        /// <summary>
        /// Parses the birth and survive rules of a cellular automaton.
        /// The rule uses the format B[..]/S[..] where [..] is the valid neighbours counts.
        /// Each neighbours count is split by a comma (,), and chains can be marked using a hyphen (-).
        /// For instance, B1-3/S7,8 means cells are born if there are 1, 2 or 3 neighbours, and survive if there are 7 or 8 neighbours.
        /// Returns a Vector2Int. Each birth neighbours count is converted using 2^n formula and added to the x value. Same for survive neighbours count with y value.
        /// To check a cell, simply count its neighbours, convert it using the same 2^n formula and compare it to the rule using a AND operator.
        /// </summary>
        /// <param name="rules">Ruleset string (B[..]/S[..] where [..] is the valid neighbours counts).</param>
        /// <param name="log">Logs the parsed ruleset to Unity console.</param>
        /// <returns>Rules as a 4 ints Tuple format.</returns>
        public static (int, int, int, int) ParseRuleset(string rules, bool log = false)
        {
            try
            {
                rules = rules.ToUpper();

                // Split birth and survive rules.
                string[] rulesSplit = rules.Split('/');

                int surviveRule = ParseNeighboursRule(rulesSplit[0]);
                int birthRule = ParseNeighboursRule(rulesSplit[1]);
                int cellStatesRule = rulesSplit.Length >= 3 ? ParseCellStatesRule(rulesSplit[2]) : 2;
                int neighbourhoodRule = rulesSplit.Length >= 4 ? ParseNeighbourhoodRule(rulesSplit[3]) : 0;

                if (log)
                {
                    Debug.Log($"Survive: {rulesSplit[0]} ({surviveRule})"
                              + $"\nBirth: {rulesSplit[1]} ({birthRule})"
                              + $"\nStates: {cellStatesRule}"
                              + $"\nNeighbourhood: {NeighbourhoodToString(neighbourhoodRule)}");
                }
                
                return (surviveRule, birthRule, cellStatesRule, neighbourhoodRule);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Could not parse rules {rules}! Make sure rules format is B[x]/S[x].\nException message: {e}");
                return (0, 0, 0, 0);
            }
        }

        private static int ParseNeighboursRule(string rule)
        {
            if (rule.Length == 0)
                return -1;
            
            string[] ruleChains = rule.Split(',');
            int ruleToInt = 0;

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

        private static int ParseCellStatesRule(string rule)
        {
            return int.TryParse(rule, out int ruleToInt) ? ruleToInt : 2;
        }

        private static int ParseNeighbourhoodRule(string rule)
        {
            return rule.ToUpper() switch
            {
                "M" => 0,
                "N" => 1,
                _ => 0,
            };
        }

        private static string NeighbourhoodToString(int neighbourhood)
        {
            return neighbourhood switch
            {
                0 => "Moore",
                1 => "Neumann",
                _ => "Moore",
            };
        }
    }
}