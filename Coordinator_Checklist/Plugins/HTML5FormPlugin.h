//
//  HTML5FormPlugin.h
//  Coordinator_Checklist
//
//  Created by Bietz on 12/14/12.
//
//

#import <Foundation/Foundation.h>
#import <Cordova/CDVPlugin.h>
#import <UIKit/UIKit.h>
#import <MessageUI/MessageUI.h>

@interface HTML5FormPlugin : CDVPlugin <MFMailComposeViewControllerDelegate> {
    NSString *callbackID;
}
@property (nonatomic, copy) NSString *callbackID;

//Instance Method
- (void)logFormData:(NSMutableArray*)arguments withDict:(NSMutableDictionary*)options;

- (void)sendResults:(NSMutableArray *)arguments withDict:(NSMutableDictionary *)options;

@end
