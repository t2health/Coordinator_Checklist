/*
 *
 * CoordinatorChecklist
 *
 * Copyright © 2009-2012 United States Government as represented by
 * the Chief Information Officer of the National Center for Telehealth
 * and Technology. All Rights Reserved.
 *
 * Copyright © 2009-2012 Contributors. All Rights Reserved.
 *
 * THIS OPEN SOURCE AGREEMENT ("AGREEMENT") DEFINES THE RIGHTS OF USE,
 * REPRODUCTION, DISTRIBUTION, MODIFICATION AND REDISTRIBUTION OF CERTAIN
 * COMPUTER SOFTWARE ORIGINALLY RELEASED BY THE UNITED STATES GOVERNMENT
 * AS REPRESENTED BY THE GOVERNMENT AGENCY LISTED BELOW ("GOVERNMENT AGENCY").
 * THE UNITED STATES GOVERNMENT, AS REPRESENTED BY GOVERNMENT AGENCY, IS AN
 * INTENDED THIRD-PARTY BENEFICIARY OF ALL SUBSEQUENT DISTRIBUTIONS OR
 * REDISTRIBUTIONS OF THE SUBJECT SOFTWARE. ANYONE WHO USES, REPRODUCES,
 * DISTRIBUTES, MODIFIES OR REDISTRIBUTES THE SUBJECT SOFTWARE, AS DEFINED
 * HEREIN, OR ANY PART THEREOF, IS, BY THAT ACTION, ACCEPTING IN FULL THE
 * RESPONSIBILITIES AND OBLIGATIONS CONTAINED IN THIS AGREEMENT.
 *
 * Government Agency: The National Center for Telehealth and Technology
 * Government Agency Original Software Designation:  * CoordinatorChecklist001
 * Government Agency Original Software Title:  * CoordinatorChecklist
 * User Registration Requested. Please send email
 * with your contact information to: robert.kayl2@us.army.mil
 * Government Agency Point of Contact for Original Software: robert.kayl2@us.army.mil
 *
 */

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
