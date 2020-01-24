/*
 *  Copyright 2016 The WebRTC Project Authors. All rights reserved.
 *
 *  Use of this source code is governed by a BSD-style license
 *  that can be found in the LICENSE file in the root of the source
 *  tree. An additional intellectual property rights grant can be found
 *  in the file PATENTS.  All contributing project authors may
 *  be found in the AUTHORS file in the root of the source tree.
 */

#import <Foundation/Foundation.h>

#import "RTCMacros.h"

NS_ASSUME_NONNULL_BEGIN

RTC_OBJC_EXPORT
@interface RTCTracking : NSObject

- (instancetype)init NS_UNAVAILABLE;

+ (void) RTCSetupInternalTracer;

+ (BOOL) RTCStartInternalCapture:(NSString*) filePath;

+ (void) RTCStopInternalCapture;

+ (void) RTCShutdownInternalTracer;

@end

NS_ASSUME_NONNULL_END