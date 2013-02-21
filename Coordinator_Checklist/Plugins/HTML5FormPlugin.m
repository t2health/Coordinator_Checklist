//
//  HTML5FormPlugin.m
//  Coordinator_Checklist
//
//  Created by Bietz on 12/14/12.
//
//

#import "HTML5FormPlugin.h"
#import "FormLogController.h"
#import "StudyLogController.h"

@implementation HTML5FormPlugin
@synthesize callbackID;

-(void)logFormData:(NSMutableArray *)arguments withDict:(NSMutableDictionary *)options {
    //The first argument in the arguments parameter is the callbackID.
    //We use this to send data back to the successCallback or failureCallback
    //through PluginResult
    self.callbackID = [arguments pop];
    
    //Get the string that javascript sent us
    NSString *stringObtainedFromJavascript = [NSString stringWithFormat:@"%@", [arguments objectAtIndex:0]];
    
    //Create the message that we wish to send to the javascript
    NSMutableString *stringToReturn = [NSMutableString stringWithString:@"StringReceived:"];
    [stringToReturn appendString:stringObtainedFromJavascript];
    //Create Plugin Result
    CDVPluginResult* pluginFormResult = [CDVPluginResult resultWithStatus:CDVCommandStatus_OK
                                                messageAsString:[NSString stringWithFormat:@"%@", [arguments objectAtIndex:0]]];
    
#ifdef DEBUG
    NSLog(@"HTML5FormPlugin called with action: %@", stringObtainedFromJavascript );
#endif
    //Log event in analytics.
    [FormLogController logFormData:stringObtainedFromJavascript];
    NSString *enrollEmail = [[NSUserDefaults standardUserDefaults] valueForKey:@"studyEmail"];
    NSString *participantId = [[NSUserDefaults standardUserDefaults] valueForKey:@"participantNumber"];
    if([stringObtainedFromJavascript isEqualToString:@"_mailit_"] && enrollEmail && participantId) {
        StudyLogController *mailit = [[StudyLogController alloc] init];
        [mailit mailStudy:participantId withEmail:enrollEmail];
    }
    [self writeJavascript: [pluginFormResult toSuccessCallbackString:self.callbackID]];
}

- (void)sendResults:(NSMutableArray *)arguments withDict:(NSMutableDictionary *)options
{
    if ([MFMailComposeViewController canSendMail])
    {
        MFMailComposeViewController *mailer = [[MFMailComposeViewController alloc] init];
        mailer.mailComposeDelegate = self;
        [mailer setSubject:@"Results"];
        NSArray *toRecipients = [NSArray arrayWithObjects:@"wes.turney@tee2.org", nil];
        [mailer setToRecipients:toRecipients];
        NSString *json = [arguments objectAtIndex:0];
        [mailer addAttachmentData:[json dataUsingEncoding:NSUTF8StringEncoding] mimeType:@"text/plain" fileName:@"results.json"];
        NSString *emailBody = @"Results attached.";
        [mailer setMessageBody:emailBody isHTML:NO];
        [[super viewController] presentModalViewController:mailer animated:YES];
        [mailer release];
    }
    else
    {
        UIAlertView *alert = [[UIAlertView alloc] initWithTitle:@"Failure"
                                                        message:@"Your device doesn't support the composer sheet"
                                                       delegate:nil
                                              cancelButtonTitle:@"OK"
                                              otherButtonTitles:nil];
        [alert show];
        [alert release];
    }
    NSLog(@"Nothing to send lols");
}

- (void)mailComposeController:(MFMailComposeViewController *)controller didFinishWithResult:(MFMailComposeResult)result error:(NSError *)error
{
    [[super viewController] dismissViewControllerAnimated:true completion:^{
        
    }];
}
@end
