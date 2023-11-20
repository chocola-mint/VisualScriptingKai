using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TestTools;

namespace CHM.VisualScriptingPlus.Editor.Tests
{
    public class Test_GraphUtility
    {
        // A Test behaves as an ordinary method
        [Test]
        public void TestGraphQueries()
        {
            var testcases = GraphUtility.FindAllGraphAssets(
                new string[]{ "Assets/VisualScriptingPlus/Tests/Editor" });
            Assert.That(
                testcases.Select(x => x.Name), 
                Is.EquivalentTo(new HashSet<string>{
                    "TestcaseScriptGraph1",
                    "TestcaseScriptGraph2",
                    "TestcaseScriptGraph3",
                    "TestcaseStateGraph1",
                }));
            Assert.That(
                GraphUtility.FindNodes(testcases, "Add").Count(),
                Is.EqualTo(4));
            Assert.That(
                GraphUtility.FindNodes(testcases, "Multiply").Count(),
                Is.EqualTo(2));
            Assert.That(
                GraphUtility.FindStickyNotes(testcases, "Test TODO").Count(),
                Is.EqualTo(2));
            Assert.That(
                GraphUtility.FindStickyNotes(testcases, "Test TODO 1").Count(),
                Is.EqualTo(1));
        }
    }
}
