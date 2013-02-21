//
//  StudyLogController.m
//  Coordninator Checklist
//
//  Created by Bietz on 12/14/12.
//
//

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
