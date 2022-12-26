#import <Foundation/Foundation.h>
#import <GameKit/GameKit.h>

typedef void (*IdentityVerificationSignatureCallback)(const char * publicKeyUrl, const char * signature, int signatureLength, const char * salt, int saltLength, const uint64_t timestamp, const char * error);

extern void generateIdentityVerificationSignature(IdentityVerificationSignatureCallback callback) {
    
    GKLocalPlayer * localPlayer = [GKLocalPlayer localPlayer];

    NSLog(@"LocalPlayer: %@", localPlayer.playerID);
    
    [localPlayer fetchItemsForIdentityVerificationSignature:^(NSURL * _Nullable publicKeyURL, NSData * _Nullable signature, NSData * _Nullable salt, uint64_t timestamp, NSError * _Nullable error) {

        NSLog(@"Received 'generateIdentityVerificationSignature' callback, error: %@", error.description);

        // Create a pool for releasing the resources we create
        @autoreleasepool {
            
            // PublicKeyUrl
            const char * publicKeyUrlCharPointer = NULL;
            if (publicKeyURL != NULL)
            {
                const NSString * publicKeyUrlString = [[NSString alloc] initWithString:[publicKeyURL absoluteString]];
                publicKeyUrlCharPointer = [publicKeyUrlString UTF8String];
            }
            
            // Signature
            const char * signatureBytes = (char*)[signature bytes];
            int signatureLength = (int)[signature length];

            // Salt
            const char * saltBytes = (char*)[salt bytes];
            int saltLength = (int)[salt length];

            // Error
            const NSString * errorString = error.description;
            const char * errorStringPointer = [errorString UTF8String];

            // Callback
            callback(publicKeyUrlCharPointer, signatureBytes, signatureLength, saltBytes, saltLength, timestamp, errorStringPointer);
        }
    }];
}