using HelixWatchdog.Core.Models;
using HelixWatchdog.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HelixWatchdog.Tests
{
    [TestClass]
    public class HelixFactoryTest
    {
        [TestMethod]
        public void GetModule_ValidPaths()
        {
            var factory = new HelixFactory();

            HelixModule module1 = factory.GetModule("Foundation\\MyFoundation");
            HelixModule module2 = factory.GetModule("FEATURE\\MyFeature");
            HelixModule module3 = factory.GetModule("Project\\MyProject");
            HelixModule module4 = factory.GetModule("WebsIte\\MyWebsite");
            HelixModule module5 = factory.GetModule("Feature\\2 Project\\Sub Folder");

            Assert.AreEqual(HelixLayer.Foundation, module1.Layer);
            Assert.AreEqual("MyFoundation", module1.Name);
            Assert.AreEqual(HelixLayer.Feature, module2.Layer);
            Assert.AreEqual("MyFeature", module2.Name);
            Assert.AreEqual(HelixLayer.Project, module3.Layer);
            Assert.AreEqual("MyProject", module3.Name);
            Assert.AreEqual(HelixLayer.Website, module4.Layer);
            Assert.AreEqual("MyWebsite", module4.Name);
            Assert.AreEqual(HelixLayer.Feature, module5.Layer);
            Assert.AreEqual("2 Project", module5.Name);
        }

        [TestMethod]
        public void GetModule_InvalidPaths()
        {
            var factory = new HelixFactory();

            HelixModule module1 = factory.GetModule("Feature");
            HelixModule module2 = factory.GetModule("Something\\Undefined Module\\");

            Assert.AreEqual(HelixLayer.Feature, module1.Layer);
            Assert.AreEqual(null, module1.Name);

            Assert.AreEqual(HelixLayer.Undefined, module2.Layer);
            Assert.AreEqual("Undefined Module", module2.Name);
        }

        [TestMethod]
        public void GetReference_ValidName()
        {
            var factory = new HelixFactory();

            HelixReference ref1 = factory.GetReference("Feature.MyProject");
            HelixReference ref2 = factory.GetReference("Project.MyProjectModule.Some.Name.Space");

            Assert.AreEqual(HelixLayer.Feature, ref1.Layer);
            Assert.AreEqual("MyProject", ref1.ModuleName);

            Assert.AreEqual(HelixLayer.Project, ref2.Layer);
            Assert.AreEqual("MyProjectModule", ref2.ModuleName);
        }

        [TestMethod]
        public void ExtractReferences()
        {
            var factory = new HelixFactory();

            var content = @"
                            <setting name='Test.Name.Space.Feature.SomeModule.SomeSetting' />
                            using Test.Name.Space.Project.OtherModule.Class1;
                            using OtherNameSpace.Foundation.Class2; // should not match
                            <ProjectReference Include='..\..\..\Foundation\MyModule\code\Test.Name.Space.Foundation.MyModule.csproj'>
                    ";

            var references = factory.ExtractReferences(content, "Test.Name.Space");
            
            Assert.AreEqual(3, references.Count);
            Assert.AreEqual(HelixLayer.Feature, references[0].Layer);
            Assert.AreEqual("SomeModule", references[0].ModuleName);

            Assert.AreEqual(HelixLayer.Project, references[1].Layer);
            Assert.AreEqual("OtherModule", references[1].ModuleName);

            Assert.AreEqual(HelixLayer.Foundation, references[2].Layer);
            Assert.AreEqual("MyModule", references[2].ModuleName);
        }

    }
}
