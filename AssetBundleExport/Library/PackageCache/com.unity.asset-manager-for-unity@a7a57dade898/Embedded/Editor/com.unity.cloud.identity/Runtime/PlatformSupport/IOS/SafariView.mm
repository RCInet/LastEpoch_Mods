#import <SafariServices/SafariServices.h>
 
extern UIViewController * UnityGetGLViewController();
 
extern "C"
{
  @interface SafariViewController : UIViewController<SFSafariViewControllerDelegate>
  @end
 
  @implementation SafariViewController
  - (void)safariViewControllerDidFinish:(SFSafariViewController *)controller {
    NSLog(@"safariViewControllerDidFinish");
  }
  @end
 
  
  SafariViewController * svc;

  void LaunchCaptiveSafariWebViewUrl(const char * url)
  {
    NSLog(@"Launching SFSafariViewController");

    // Get the instance of ViewController that Unity is displaying now
    UIViewController * uvc = UnityGetGLViewController();

    NSMutableString *urlMutableString = [NSMutableString stringWithString:[[NSString alloc] initWithUTF8String:url]];
      
    // Generate an NSURL object based on the C string passed from C#
    NSURL * URL = [NSURL URLWithString: urlMutableString];

    // Create an SFSafariViewController object from the generated URL
    SFSafariViewController * sfvc = [[SFSafariViewController alloc] initWithURL:URL];

    // Assign a delegate to handle when the user presses the 'Done' button
    svc = [[SafariViewController alloc] init];
    sfvc.delegate = svc;

    // Start the generated SFSafariViewController object
    [uvc presentViewController:sfvc animated:YES completion:nil];

    NSLog(@"Presented SFSafariViewController");
  }

  void DismissCaptiveSafariWebView()
  {
    NSLog(@"DismissCaptiveSafariWebView");
    UIViewController * uvc = UnityGetGLViewController();
    [uvc dismissViewControllerAnimated:YES completion:nil];
  }
  
}