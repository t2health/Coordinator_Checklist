/*
 Licensed to the Apache Software Foundation (ASF) under one
 or more contributor license agreements.  See the NOTICE file
 distributed with this work for additional information
 regarding copyright ownership.  The ASF licenses this file
 to you under the Apache License, Version 2.0 (the
 "License"); you may not use this file except in compliance
 with the License.  You may obtain a copy of the License at

 http://www.apache.org/licenses/LICENSE-2.0

 Unless required by applicable law or agreed to in writing,
 software distributed under the License is distributed on an
 "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 KIND, either express or implied.  See the License for the
 specific language governing permissions and limitations
 under the License.
 */

//
//  MainViewController.h
//  Coordinator_Checklist
//
//  Created by ___FULLUSERNAME___ on ___DATE___.
//  Copyright ___ORGANIZATIONNAME___ ___YEAR___. All rights reserved.
//

#import "MainViewController.h"
#import "AppDelegate.h"
#import <CoreData/CoreData.h>
#import <AssetsLibrary/AssetsLibrary.h>
#import "ViewReaderController.h"

@implementation MainViewController

@synthesize assets = _assets;

- (id)initWithNibName:(NSString*)nibNameOrNil bundle:(NSBundle*)nibBundleOrNil
{
    self = [super initWithNibName:nibNameOrNil bundle:nibBundleOrNil];
    if (self) {
        // Custom initialization
    }
    return self;
}

- (void)didReceiveMemoryWarning
{
    // Releases the view if it doesn't have a superview.
    [super didReceiveMemoryWarning];

    // Release any cached data, images, etc that aren't in use.
}

#pragma mark - View lifecycle

- (void)viewWillAppear:(BOOL)animated
{
    // Set the main view to utilize the entire application frame space of the device.
    // Change this to suit your view's UI footprint needs in your application.

    UIView* rootView = [[[[UIApplication sharedApplication] keyWindow] rootViewController] view];
    CGRect webViewFrame = [[[rootView subviews] objectAtIndex:0] frame];  // first subview is the UIWebView

    if (CGRectEqualToRect(webViewFrame, CGRectZero)) { // UIWebView is sized according to its parent, here it hasn't been sized yet
        self.view.frame = [[UIScreen mainScreen] applicationFrame]; // size UIWebView's parent according to application frame, which will in turn resize the UIWebView
    }

    [super viewWillAppear:animated];
}

- (void)viewDidLoad
{
    [super viewDidLoad];
    // Do any additional setup after loading the view from its nib.
}

- (void)viewDidUnload
{
    [super viewDidUnload];
    // Release any retained subviews of the main view.
    // e.g. self.myOutlet = nil;
}

- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation
{
    // Return YES for supported orientations
    return [super shouldAutorotateToInterfaceOrientation:interfaceOrientation];
}

/* Comment out the block below to over-ride */

/*
- (CDVCordovaView*) newCordovaViewWithFrame:(CGRect)bounds
{
    return[super newCordovaViewWithFrame:bounds];
}
*/

/* Comment out the block below to over-ride */

/*
#pragma CDVCommandDelegate implementation

- (id) getCommandInstance:(NSString*)className
{
    return [super getCommandInstance:className];
}

- (BOOL) execute:(CDVInvokedUrlCommand*)command
{
    return [super execute:command];
}

- (NSString*) pathForResource:(NSString*)resourcepath;
{
    return [super pathForResource:resourcepath];
}

- (void) registerPlugin:(CDVPlugin*)plugin withClassName:(NSString*)className
{
    return [super registerPlugin:plugin withClassName:className];
}
*/

#pragma UIWebDelegate implementation

/**
 Called when the webview finishes loading.  This stops the activity view and closes the imageview
 */
- (void)webViewDidFinishLoad:(UIWebView *)theWebView
{
    static int shown = 0;
	// only valid if lifearmor.plist specifies a protocol to handle
	if(self.invokeString)
	{
        NSLog(@"DEPRECATED: window.invokeString - use the window.handleOpenURL(url) function instead, which is always called when the app is launched through a custom scheme url.");
		// this is passed before the deviceready event is fired, so you can access it in js when you receive deviceready
		NSString* jsString = [NSString stringWithFormat:@"var invokeString = \"%@\";", self.invokeString];
		[theWebView stringByEvaluatingJavaScriptFromString:jsString];
	}
    
    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    
    bool agreeEULA = [defaults boolForKey:@"agreeEULA"];
    if(shown == 0 && !agreeEULA) {
        [self showEULA:self.viewController];
    }
    
    // Black base color for background matches the native apps
    theWebView.backgroundColor = [UIColor blackColor];
    
	return [ super webViewDidFinishLoad:theWebView ];
}

- (void)webViewDidStartLoad:(UIWebView *)theWebView
{
	return [ super webViewDidStartLoad:theWebView ];
}

/**
 * Fail Loading With Error
 * Error - If the webpage failed to load display an error with the reason.
 */
- (void)webView:(UIWebView *)theWebView didFailLoadWithError:(NSError *)error
{
	return [ super webView:theWebView didFailLoadWithError:error ];
}

/**
 * Start Loading Request
 * This is where most of the magic happens... We take the request(s) and process the response.
 * From here we can re direct links and other protocalls to different internal methods.
 */
- (BOOL)webView:(UIWebView *)theWebView shouldStartLoadWithRequest:(NSURLRequest *)request navigationType:(UIWebViewNavigationType)navigationType
{
	return [ super webView:theWebView shouldStartLoadWithRequest:request navigationType:navigationType ];
}

- (void)dealloc
{
	[ super dealloc ];
}

- (void)showEULA:(UIViewController *)controller {
    ViewReaderController *vcReader = [ViewReaderController alloc];
	vcReader.view.alpha = 0.0;
    vcReader.title = @"EULA";
    [[UIDevice currentDevice] beginGeneratingDeviceOrientationNotifications];
    UIInterfaceOrientation interfaceOrientation =  (UIInterfaceOrientation)[UIDevice currentDevice].orientation;
	CGRect r = controller.view.frame;
    if(UIInterfaceOrientationIsLandscape(interfaceOrientation)) {
        r.size.width = r.size.height;
        r.size.height = controller.view.frame.size.width;
    } else if(interfaceOrientation == 0) {
        r.size.height = r.size.width;
    }
    vcReader.pixelsPerFrame = 1.0f;
    vcReader.animationInterval = 1.0f / 15.0f;
    if (UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad) {
        vcReader.pixelsPerFrame = 1.0f;
        vcReader.animationInterval = 1.0f / 15.0f;
    }
    vcReader.view.backgroundColor = [UIColor colorWithRed:0.0f green:0.0f blue:0.0f alpha:0.5f];
    vcReader.modalTransitionStyle = UIModalTransitionStyleFlipHorizontal;
    vcReader.modalPresentationStyle = UIModalPresentationFullScreen;
	[vcReader loadHTML:@"eula"];
	[vcReader fadeInReader];
    [controller presentModalViewController:vcReader animated:YES];
    [vcReader release];
    
}

@end
