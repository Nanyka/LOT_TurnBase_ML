using System;
using System.Collections;
using System.Collections.Generic;
using JumpeeIsland;
using Unity.Services.Authentication;
using Unity.Services.CloudCode;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;

public class TestModule : MonoBehaviour
{
    private async void Start()
    {
        // Initialize the Unity Services Core SDKvar options = new InitializationOptions();
        var options = new InitializationOptions();
        options.SetEnvironmentName("dev");
        await UnityServices.InitializeAsync(options);

        // Authenticate by logging into an anonymous account
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        string message = CreateTestEmail();

        try
        {
            // Call the function within the module and provide the parameters we defined in there
            string result = await CloudCodeService.Instance.CallModuleEndpointAsync("TestCSharpModule", "SendBattleEmail", new Dictionary<string, object>
            {
                {"playerId", "LFkwKDenufHksILroYRvQHjKFlIJ"},
                {"message", message}
            });

            Debug.Log(result);
        }
        catch (CloudCodeException exception)
        {
            Debug.LogException(exception);
        }
    }

    private string CreateTestEmail()
    {
        List<InboxMessage> inboxMessages = new();
        inboxMessages.Add(CreateATestMail(new BattleRecord()));

        var inboxBattleRecords = new InboxState
        {
            messages = inboxMessages
        };
        var inboxBattleRecordsJson = JsonUtility.ToJson(inboxBattleRecords);

        return inboxBattleRecordsJson;
    }
    
    private InboxMessage CreateATestMail(BattleRecord battleRecord)
    {
        var testBattleMessage = new InboxMessage();
        testBattleMessage.messageId = "TEST_BATTLE_MESSAGE";
        testBattleMessage.messageInfo = new MessageInfo();
        testBattleMessage.messageInfo.title = "TestMessage";
        testBattleMessage.messageInfo.content = "I create this message to test adding a new message";
        testBattleMessage.messageInfo.expiration = "0.00:03:00.00";
        testBattleMessage.battleData = new MessageBattleData();
        testBattleMessage.battleData.battleRecord = battleRecord;

        var expirationPeriod = TimeSpan.Parse(testBattleMessage.messageInfo.expiration);
        var hasUnclaimedAttachment = !string.IsNullOrEmpty(testBattleMessage.messageInfo.attachment);

        testBattleMessage.metadata = new MessageMetadata(expirationPeriod, hasUnclaimedAttachment);
        return testBattleMessage;
    }
    
    [Serializable]
    struct InboxState
    {
        public List<InboxMessage> messages;
    }
}
