//
//  LogFormData.h
//  Coordninator Checklist
//
//  Created by Bietz on 12/14/12.
//
//

#import <Foundation/Foundation.h>
#import <CoreData/CoreData.h>


@interface LogFormData : NSManagedObject

@property (nonatomic, retain) NSString * type;
@property (nonatomic, retain) NSString * data;
@property (nonatomic, retain) NSDate * dateCreated;

@end
