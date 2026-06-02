using Jade.World;
using UnityEditor;
using UnityEngine;

namespace Jade.EditorTools
{
    public static class ShanhaiGateGrayboxValidationTool
    {
        [MenuItem("Jade/Validate Shanhai Gate Graybox Reachability")]
        public static void ValidateReachability()
        {
            ShanhaiGateGrayboxReachabilityReport report = ShanhaiGateGrayboxReachability.Validate();
            if (!report.IsPassing)
            {
                Debug.LogError(
                    "Shanhai Gate graybox reachability failed. "
                    + "DashGate=" + report.NoAbilityStopsAtDash
                    + ", DashToDoubleJump=" + report.DashReachesDoubleJump
                    + ", DashCannotFinal=" + report.DashCannotReachFinal
                    + ", DoubleJumpAllShards=" + report.DoubleJumpReachesAllShards
                    + ", DoubleJumpFinal=" + report.DoubleJumpReachesFinal
                    + ", GateRequiresFour=" + report.GateRequiresAllFour);
                return;
            }

            Debug.Log("Shanhai Gate graybox reachability passed: dash -> double jump -> four shards -> gate.");
        }
    }
}
