using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace CHM.VisualScriptingPlus.Editor.Tests
{
    public class Test_GraphUtility
    {
        const string TestDirectory = "Packages/com.chocola-mint.visual-scripting-plus/Tests/Editor/";
        [Test]
        public void TestGraphQueries()
        {
            // We use scenes instead of graph assets so users won't see testcase graphs in the fuzzy search.
            var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(TestDirectory + "TestScene");
            AssetDatabase.OpenAsset(sceneAsset);
            // Now we've opened the test scene, this will find all graph scripts in the scene.
            var testcases = GraphUtility.FindAllRuntimeGraphSources();
            Assert.That(
                testcases.Count(), Is.EqualTo(2));
            // In MyStateMachine.
            Assert.That(
                GraphUtility.FindNodes(testcases, "Add").Count(),
                Is.EqualTo(1));
            // In MyScriptMachine.
            Assert.That(
                GraphUtility.FindNodes(testcases, "Assert").Count(),
                Is.EqualTo(1));
            Assert.That(
                GraphUtility.FindStickyNotes(testcases, "TODO").Count(),
                Is.EqualTo(3));
            Assert.That(
                GraphUtility.FindStickyNotes(testcases, "TODO 1").Count(),
                Is.EqualTo(1));
            Assert.That(
                GraphUtility.FindStates(testcases, "State").Count(),
                Is.EqualTo(4 + 1)); // There's one in the Script Machine as well.
            Assert.That(
                GraphUtility.FindStateTransitions(testcases, "Transition").Count(),
                Is.EqualTo(2));
        }
    }
}
