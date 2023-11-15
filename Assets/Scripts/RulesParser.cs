namespace CellularAutomata
{
    using UnityEngine;

    public static class RulesParser
    {
        /// <summary>
        /// Parses the birth and survive rules of a cellular automaton.
        /// The rule uses the format B[..]/S[..] where [..] is the valid neighbours counts.
        /// For instance, B123/S7 means cells are born if there are 1, 2 or 3 neighbours, and survive if there are 7 neighbours.
        /// Returns a Vector2Int. Each birth neighbours count is converted using 2^n formula and added to the x value. Same for survive neighbours count with y value.
        /// To check a cell, simply count its neighbours, convert it using the same 2^n formula and compare it to the rule using a AND operator.
        /// </summary>
        /// <param name="rules">Ruleset string (B[..]/S[..] where [..] is the valid neighbours counts).</param>
        /// <returns>Rules as a Vector2Int format.</returns>
        public static Vector2Int Parse(string rules)
        {
            Vector2Int parsedRules = Vector2Int.zero;
            rules = rules.ToUpper();

            try
            {
                // Split birth and survive rules.
                string[] rulesSplit = rules.Split('/');
                string birthRules = rulesSplit[0];
                string surviveRules = rulesSplit[1];

                // Parse birth rules. Start at 1 to ignore 'B' prefix.
                for (int i = 1; i < birthRules.Length; ++i)
                {
                    int neighboursCount = int.Parse(birthRules[i].ToString()); // Parse value.
                    neighboursCount = (int)Mathf.Pow(2, neighboursCount); // Convert value to 2^value.
                    parsedRules.x += neighboursCount; // Add value to birth rule.
                }

                // Parse birth rules. Start at 1 to ignore 'S' prefix.
                for (int i = 1; i < surviveRules.Length; ++i)
                {
                    int neighboursCount = int.Parse(surviveRules[i].ToString()); // Parse value.
                    neighboursCount = (int)Mathf.Pow(2, neighboursCount); // Convert value to 2^value.
                    parsedRules.y += neighboursCount; // Add value to survive rule.
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Could not parse rules {rules}! Make sure rules format is B[x]/S[x].\nException message: {e}");
                parsedRules = Vector2Int.zero;
            }

            return parsedRules;
        }
    }
}