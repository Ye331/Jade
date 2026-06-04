using System.Collections.Generic;

namespace Jade.World
{
    public static class ShanhaiGateGrayboxReachability
    {
        public static ShanhaiGateGrayboxReachabilityReport Validate()
        {
            bool noAbilityStopsAtDash = !CanReach("S1_DashGate_Landing", AbilityFlags.None);
            bool dashReachesDoubleJump = CanReach("Ability_DoubleJump", AbilityFlags.Dash);
            bool dashCannotReachFinal = !CanReach("FinalGate", AbilityFlags.Dash);
            bool doubleJumpReachesAllShards = CanReachAllShards(AbilityFlags.Dash | AbilityFlags.DoubleJump);
            bool doubleJumpReachesFinal = CanReach("FinalGate", AbilityFlags.Dash | AbilityFlags.DoubleJump);
            bool gateRequiresAllFour = RequiredShardCount == 4;

            return new ShanhaiGateGrayboxReachabilityReport(
                noAbilityStopsAtDash,
                dashReachesDoubleJump,
                dashCannotReachFinal,
                doubleJumpReachesAllShards,
                doubleJumpReachesFinal,
                gateRequiresAllFour);
        }

        private const int RequiredShardCount = 4;

        private static bool CanReachAllShards(AbilityFlags abilities)
        {
            return CanReach("Shard_01_ReturnAfterDoubleJump", abilities)
                && CanReach("Shard_02_BambooCanopy", abilities)
                && CanReach("Shard_03_FinalClimbUpper", abilities)
                && CanReach("Shard_04_FinalHighLedge", abilities);
        }

        private static bool CanReach(string target, AbilityFlags abilities)
        {
            Queue<string> open = new Queue<string>();
            HashSet<string> visited = new HashSet<string>();
            open.Enqueue("Start");
            visited.Add("Start");

            while (open.Count > 0)
            {
                string current = open.Dequeue();
                if (current == target)
                {
                    return true;
                }

                List<RouteEdge> edges = GetEdges(current);
                for (int i = 0; i < edges.Count; i++)
                {
                    RouteEdge edge = edges[i];
                    if ((abilities & edge.RequiredAbilities) != edge.RequiredAbilities || visited.Contains(edge.To))
                    {
                        continue;
                    }

                    visited.Add(edge.To);
                    open.Enqueue(edge.To);
                }
            }

            return false;
        }

        private static List<RouteEdge> GetEdges(string node)
        {
            switch (node)
            {
                case "Start":
                    return Edges(("S1_DashPickup", AbilityFlags.None));
                case "S1_DashPickup":
                    return Edges(
                        ("S1_DashGate_Landing", AbilityFlags.Dash),
                        ("Ability_DoubleJump", AbilityFlags.Dash));
                case "S1_DashGate_Landing":
                    return Edges(("Ability_DoubleJump", AbilityFlags.None));
                case "Ability_DoubleJump":
                    return Edges(
                        ("Shard_01_ReturnAfterDoubleJump", AbilityFlags.DoubleJump),
                        ("S2_LakeEntry", AbilityFlags.DoubleJump));
                case "Shard_01_ReturnAfterDoubleJump":
                    return Edges(("S2_LakeEntry", AbilityFlags.DoubleJump));
                case "S2_LakeEntry":
                    return Edges(
                        ("Shard_02_BambooCanopy", AbilityFlags.DoubleJump),
                        ("S3_FinalClimb", AbilityFlags.DoubleJump));
                case "Shard_02_BambooCanopy":
                    return Edges(("S3_FinalClimb", AbilityFlags.DoubleJump));
                case "S3_FinalClimb":
                    return Edges(
                        ("Shard_03_FinalClimbUpper", AbilityFlags.DoubleJump),
                        ("Shard_04_FinalHighLedge", AbilityFlags.DoubleJump),
                        ("FinalGate", AbilityFlags.DoubleJump));
                case "Shard_03_FinalClimbUpper":
                    return Edges(("FinalGate", AbilityFlags.DoubleJump));
                case "Shard_04_FinalHighLedge":
                    return Edges(("FinalGate", AbilityFlags.DoubleJump));
                default:
                    return new List<RouteEdge>();
            }
        }

        private static List<RouteEdge> Edges(params (string to, AbilityFlags required)[] edges)
        {
            List<RouteEdge> result = new List<RouteEdge>(edges.Length);
            for (int i = 0; i < edges.Length; i++)
            {
                result.Add(new RouteEdge(edges[i].to, edges[i].required));
            }

            return result;
        }

        private readonly struct RouteEdge
        {
            public RouteEdge(string to, AbilityFlags requiredAbilities)
            {
                To = to;
                RequiredAbilities = requiredAbilities;
            }

            public string To { get; }
            public AbilityFlags RequiredAbilities { get; }
        }
    }

    public readonly struct ShanhaiGateGrayboxReachabilityReport
    {
        public ShanhaiGateGrayboxReachabilityReport(
            bool noAbilityStopsAtDash,
            bool dashReachesDoubleJump,
            bool dashCannotReachFinal,
            bool doubleJumpReachesAllShards,
            bool doubleJumpReachesFinal,
            bool gateRequiresAllFour)
        {
            NoAbilityStopsAtDash = noAbilityStopsAtDash;
            DashReachesDoubleJump = dashReachesDoubleJump;
            DashCannotReachFinal = dashCannotReachFinal;
            DoubleJumpReachesAllShards = doubleJumpReachesAllShards;
            DoubleJumpReachesFinal = doubleJumpReachesFinal;
            GateRequiresAllFour = gateRequiresAllFour;
        }

        public bool NoAbilityStopsAtDash { get; }
        public bool DashReachesDoubleJump { get; }
        public bool DashCannotReachFinal { get; }
        public bool DoubleJumpReachesAllShards { get; }
        public bool DoubleJumpReachesFinal { get; }
        public bool GateRequiresAllFour { get; }

        public bool IsPassing =>
            NoAbilityStopsAtDash
            && DashReachesDoubleJump
            && DashCannotReachFinal
            && DoubleJumpReachesAllShards
            && DoubleJumpReachesFinal
            && GateRequiresAllFour;
    }

    [System.Flags]
    public enum AbilityFlags
    {
        None = 0,
        Dash = 1,
        DoubleJump = 2
    }
}
