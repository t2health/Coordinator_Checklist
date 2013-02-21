//
//  StudyLogController.h
//  Coordninator Checklist
//
//  Created by Bietz on 12/14/12.
//
//

#import <UIKit/UIKit.h>
#import <MessageUI/MessageUI.h>

@interface StudyLogController :NSObject <MFMailComposeViewControllerDelegate> {
    
}
- (void)mailStudy:(NSString *)participantID withEmail:(NSString *)emailAddress;
@end
