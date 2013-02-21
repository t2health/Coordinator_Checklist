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

#import "StudyLogController.h"
#import "AppDelegate.h"
#import "LogFormData.h"

@implementation StudyLogController


- (void)mailStudy:(NSString *)participantID withEmail:(NSString *)emailAddress {
    AppDelegate *appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
    NSManagedObjectContext *managedObjectContext = appDelegate.managedObjectContext;
    NSFetchRequest *fetchRequest = [[NSFetchRequest alloc] init];
    NSEntityDescription *entity = [NSEntityDescription entityForName:@"LogFormData" inManagedObjectContext:managedObjectContext];
    [fetchRequest setEntity:entity];
    
    [fetchRequest setFetchBatchSize:100];
    
    NSSortDescriptor *orderSortDescriptor = [[NSSortDescriptor alloc] initWithKey:@"dateCreated" ascending:YES];
    NSArray *sortDescriptors = [NSArray arrayWithObjects:orderSortDescriptor, nil];
    
    [fetchRequest setSortDescriptors:sortDescriptors];
    
    NSFetchedResultsController *fetchedResultsController = [[NSFetchedResultsController alloc] initWithFetchRequest:fetchRequest managedObjectContext:managedObjectContext sectionNameKeyPath:nil cacheName:@"Master"];
    
    NSError *error = nil;
    if (![fetchedResultsController performFetch:&error]) {
        NSLog(@"Unresolved error %@, %@", error, [error userInfo]);
        abort();
    }
    
    NSString *dir = NSTemporaryDirectory();
    NSString *file = [NSString stringWithFormat:@"%@/Coordnator_Checklist.csv", dir];
    NSMutableString *csv = [[NSMutableString alloc] init];
    for (LogFormData *entry in fetchedResultsController.fetchedObjects) {
        [csv appendFormat:@"%@, %@, %@\n", entry.dateCreated, entry.type, entry.data];
    }
    
    [csv writeToFile:file atomically:YES encoding:NSStringEncodingConversionAllowLossy error:nil];
    
    NSLog(@"docs: %@", [[NSFileManager defaultManager] contentsOfDirectoryAtPath:dir error:nil]);
    
    if ([MFMailComposeViewController canSendMail]) {
        MFMailComposeViewController *mail = [[MFMailComposeViewController alloc] init];
        mail.mailComposeDelegate = self;
        [mail setSubject:[NSString stringWithFormat: @"DoD/VA Coordnator Checklist Form Attached for %@", participantID]];
        [mail setMessageBody:@"Attached is a Coordnator Checklist" isHTML:NO];
        [mail setToRecipients:[NSArray arrayWithObject:emailAddress]];
        [mail addAttachmentData:[NSData dataWithContentsOfFile:file] mimeType:@"text/csv" fileName:@"coordinator_checklist.csv"];
        [appDelegate.viewController presentModalViewController:mail animated:YES];
    }
    
}

- (void)mailComposeController:(MFMailComposeViewController *)controller
          didFinishWithResult:(MFMailComposeResult)result
                        error:(NSError *)error {
    AppDelegate *appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
    [appDelegate.viewController dismissModalViewControllerAnimated:YES];
    
}


@end
