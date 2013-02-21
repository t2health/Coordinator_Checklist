//
//  FormLogController.h
//  Coordninator Checklist
//
//  Created by Bietz on 12/14/12.
//
//

#import <Foundation/Foundation.h>
#import "FormLogController.h"
#import "LogFormData.h"
#import "AppDelegate.h"

@interface FormLogController : NSObject {
    
}


+ (void)logFormData:(NSString *)formData;
+ (void)logFormData:(NSString *)formData withParameters:(NSDictionary *)parameters;
+ (void)logFormData:(NSString *)errorID message:(NSString *)message exception:(NSException *)exception;
+ (void)logFormData:(NSString *)errorID message:(NSString *)message error:(NSError *)error;


+ (void)logFormData:(NSString *)formData start:(NSDate *)start;

+ (void)logFormData:(NSString *)formData timed:(BOOL)timed;
+ (void)logFormData:(NSString *)formData withParameters:(NSDictionary *)parameters timed:(BOOL)timed;
+ (void)endTimedEvent:(NSString *)formData withParameters:(NSDictionary *)parameters;

@end
