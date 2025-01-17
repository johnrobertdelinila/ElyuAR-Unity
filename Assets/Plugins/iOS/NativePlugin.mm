#import <Foundation/Foundation.h>
#import <MapKit/MapKit.h>
#import <UIKit/UIKit.h>

extern "C" {
    void _OpenMapsWithAddress(const char* address, double latitude, double longitude) {
        @autoreleasepool {
            NSString *addressString = [NSString stringWithUTF8String:address];
            NSString *urlString = [NSString stringWithFormat:@"http://maps.apple.com/?address=%@&ll=%f,%f", 
                                 [addressString stringByAddingPercentEncodingWithAllowedCharacters:[NSCharacterSet URLQueryAllowedCharacterSet]],
                                 latitude, 
                                 longitude];
            NSURL *url = [NSURL URLWithString:urlString];
            
            if ([[UIApplication sharedApplication] canOpenURL:url]) {
                [[UIApplication sharedApplication] openURL:url options:@{} completionHandler:nil];
            }
        }
    }

    void _OpenURL(const char* urlStr) {
        @autoreleasepool {
            NSString *urlString = [NSString stringWithUTF8String:urlStr];
            NSURL *url = [NSURL URLWithString:urlString];
            
            if (url) {
                [[UIApplication sharedApplication] openURL:url options:@{} completionHandler:nil];
            }
        }
    }

    void _ShowShareSheet(const char* text) {
        @autoreleasepool {
            NSString *shareText = [NSString stringWithUTF8String:text];
            UIViewController *rootViewController = UnityGetGLViewController();
            
            UIActivityViewController *activityViewController = 
                [[UIActivityViewController alloc] initWithActivityItems:@[shareText] 
                                                applicationActivities:nil];
            
            [rootViewController presentViewController:activityViewController 
                                          animated:YES 
                                        completion:nil];
        }
    }
} 