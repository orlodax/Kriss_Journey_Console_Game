using System.Text.Json;
using KrissJourney.Kriss.Nodes;
using KrissJourney.Kriss.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KrissJourney.Tests
{
    [TestClass]
    public class NodeJsonConverterTests
    {
        private readonly JsonSerializerOptions _options = new()
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };

        [TestMethod]
        public void CanConvert_WithNodeBaseType_ReturnsTrue()
        {
            // Arrange
            NodeJsonConverter converter = new();

            // Act
            bool canConvert = converter.CanConvert(typeof(NodeBase));

            // Assert
            Assert.IsTrue(canConvert);
        }

        [TestMethod]
        public void CanConvert_WithActionNodeType_ReturnsTrue()
        {
            // Arrange
            NodeJsonConverter converter = new();

            // Act
            bool canConvert = converter.CanConvert(typeof(ActionNode));

            // Assert
            Assert.IsTrue(canConvert);
        }

        [TestMethod]
        public void CanConvert_WithNonNodeType_ReturnsFalse()
        {
            // Arrange
            NodeJsonConverter converter = new();

            // Act
            bool canConvert = converter.CanConvert(typeof(string));

            // Assert
            Assert.IsFalse(canConvert);
        }

        [TestMethod]
        public void Write_SerializesNodeCorrectly()
        {
            // Arrange
            ActionNode actionNode = TestUtils.CreateMockActionNode();

            // Act
            string json = JsonSerializer.Serialize(actionNode, _options);

            // Assert
            Assert.IsNotNull(json);
            Assert.IsTrue(json.Contains("\"id\":1"));
            Assert.IsTrue(json.Contains("\"text\":\"Test Action Node\""));
            Assert.IsTrue(json.Contains("\"childId\":2"));
        }

        [TestMethod]
        public void Roundtrip_ActionNode_DeserializesCorrectly()
        {
            // Arrange
            ActionNode originalNode = TestUtils.CreateMockActionNode();
            originalNode.Type = "action";

            // Act
            string json = JsonSerializer.Serialize(originalNode, _options);
            NodeBase deserializedNode = JsonSerializer.Deserialize<NodeBase>(json, _options);

            // Assert
            Assert.IsNotNull(deserializedNode);
            Assert.IsInstanceOfType(deserializedNode, typeof(ActionNode));

            ActionNode actionNode = deserializedNode as ActionNode;
            Assert.AreEqual(originalNode.Id, actionNode.Id);
            Assert.AreEqual(originalNode.Text, actionNode.Text);
            Assert.AreEqual(originalNode.ChildId, actionNode.ChildId);
            Assert.IsNotNull(actionNode.Actions);
        }

        [TestMethod]
        public void Roundtrip_ChoiceNode_DeserializesCorrectly()
        {
            // Arrange
            ChoiceNode originalNode = TestUtils.CreateMockChoiceNode();
            originalNode.Type = "choice";

            // Act
            string json = JsonSerializer.Serialize(originalNode, _options);
            NodeBase deserializedNode = JsonSerializer.Deserialize<NodeBase>(json, _options);

            // Assert
            Assert.IsNotNull(deserializedNode);
            Assert.IsInstanceOfType(deserializedNode, typeof(ChoiceNode));

            ChoiceNode choiceNode = deserializedNode as ChoiceNode;
            Assert.AreEqual(originalNode.Id, choiceNode.Id);
            Assert.AreEqual(originalNode.Text, choiceNode.Text);
            Assert.IsNotNull(choiceNode.Choices);
            Assert.AreEqual(originalNode.Choices.Count, choiceNode.Choices.Count);
        }

        [TestMethod]
        public void Roundtrip_DialogueNode_DeserializesCorrectly()
        {
            // Arrange
            DialogueNode originalNode = TestUtils.CreateMockDialogueNode();
            originalNode.Type = "dialogue";

            // Act
            string json = JsonSerializer.Serialize(originalNode, _options);
            NodeBase deserializedNode = JsonSerializer.Deserialize<NodeBase>(json, _options);

            // Assert
            Assert.IsNotNull(deserializedNode);
            Assert.IsInstanceOfType(deserializedNode, typeof(DialogueNode));

            DialogueNode dialogueNode = deserializedNode as DialogueNode;
            Assert.AreEqual(originalNode.Id, dialogueNode.Id);
            Assert.AreEqual(originalNode.Text, dialogueNode.Text);
            Assert.AreEqual(originalNode.ChildId, dialogueNode.ChildId);
            Assert.IsNotNull(dialogueNode.Dialogues);
            Assert.AreEqual(originalNode.Dialogues.Count, dialogueNode.Dialogues.Count);
        }
    }
}