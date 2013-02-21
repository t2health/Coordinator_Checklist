//
//  FormLogController.m
//  Coordninator Checklist
//
//  Created by Bietz on 12/14/12.
//
//

#import "FormLogController.h"
#import "LogFormData.h"
#import "AppDelegate.h"

static BOOL SESSION_STARTED = NO;

@implementation FormLogController

+ (void)logFormData:(NSString *)formData {
    
        AppDelegate *appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
        NSManagedObjectContext *managedObjectContext = appDelegate.managedObjectContext;
        
        LogFormData *logEntry = [NSEntityDescription insertNewObjectForEntityForName:@"LogFormData" inManagedObjectContext:managedObjectContext];
        logEntry.data = @"";
        logEntry.type = formData;
        logEntry.dateCreated = [NSDate date];
        
        [managedObjectContext save:nil];
}

+ (void)logFormData:(NSString *)formData withParameters:(NSDictionary *)parameters {
    
        AppDelegate *appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
        NSManagedObjectContext *managedObjectContext = appDelegate.managedObjectContext;
        
        LogFormData *logEntry = [NSEntityDescription insertNewObjectForEntityForName:@"LogFormData" inManagedObjectContext:managedObjectContext];
        
        NSMutableString *dataStr = [[NSMutableString alloc] init];
        BOOL first = YES;
        for (NSString *key in parameters.keyEnumerator) {
            NSObject *formData = [parameters objectForKey:key];
            if (first) {
                [dataStr appendFormat:@"%@", formData];
            } else {
                [dataStr appendFormat:@",%@", formData];
            }
            
            first = NO;
        }
        logEntry.data = dataStr;
        logEntry.type = formData;
        logEntry.dateCreated = [NSDate date];
        
        [managedObjectContext save:nil];
	
}

+ (void)logFormData:(NSString *)formData start:(NSDate *)start
{
    int seconds = [[NSDate date] timeIntervalSinceDate:start];
    int mins = seconds / 60;
    seconds = seconds % 60;
    NSDictionary *dict = [[NSDictionary alloc] initWithObjectsAndKeys:[NSString stringWithFormat:@"%i:%i", mins, seconds], @"time", nil];
    
    [FormLogController logFormData:formData withParameters:dict];
}

#pragma mark -
#pragma mark Special Analytic Functions

@end
