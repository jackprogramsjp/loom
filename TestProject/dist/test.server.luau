-- This is an automatically generated file and it should not be edited manually.
-- Generated Roblox enums
type EnumItem<Kind> = {
  read _kind: Kind & string,
  read Name: string,
  read Value: number,
  read EnumType: never,
}
type AccessModifierType = {
  read Allow: EnumItem<"AccessModifierType">,
  read Deny: EnumItem<"AccessModifierType">,
}
type AccessoryType = {
  read Unknown: EnumItem<"AccessoryType">,
  read Hat: EnumItem<"AccessoryType">,
  read Hair: EnumItem<"AccessoryType">,
  read Face: EnumItem<"AccessoryType">,
  read Neck: EnumItem<"AccessoryType">,
  read Shoulder: EnumItem<"AccessoryType">,
  read Front: EnumItem<"AccessoryType">,
  read Back: EnumItem<"AccessoryType">,
  read Waist: EnumItem<"AccessoryType">,
  read TShirt: EnumItem<"AccessoryType">,
  read Shirt: EnumItem<"AccessoryType">,
  read Pants: EnumItem<"AccessoryType">,
  read Jacket: EnumItem<"AccessoryType">,
  read Sweater: EnumItem<"AccessoryType">,
  read Shorts: EnumItem<"AccessoryType">,
  read LeftShoe: EnumItem<"AccessoryType">,
  read RightShoe: EnumItem<"AccessoryType">,
  read DressSkirt: EnumItem<"AccessoryType">,
  read Eyebrow: EnumItem<"AccessoryType">,
  read Eyelash: EnumItem<"AccessoryType">,
}
type ActionOnAutoResumeSync = {
  read DontResume: EnumItem<"ActionOnAutoResumeSync">,
  read KeepStudio: EnumItem<"ActionOnAutoResumeSync">,
  read KeepLocal: EnumItem<"ActionOnAutoResumeSync">,
}
type ActionOnStopSync = {
  read AlwaysAsk: EnumItem<"ActionOnStopSync">,
  read KeepLocalFiles: EnumItem<"ActionOnStopSync">,
  read DeleteLocalFiles: EnumItem<"ActionOnStopSync">,
}
type ActionType = {
  read Nothing: EnumItem<"ActionType">,
  read Pause: EnumItem<"ActionType">,
  read Lose: EnumItem<"ActionType">,
  read Draw: EnumItem<"ActionType">,
  read Win: EnumItem<"ActionType">,
}
type ActivePayerStatus = {
  read Unknown: EnumItem<"ActivePayerStatus">,
  read Never: EnumItem<"ActivePayerStatus">,
  read Lapsed: EnumItem<"ActivePayerStatus">,
  read Casual50Percent: EnumItem<"ActivePayerStatus">,
  read Intermediate35Percent: EnumItem<"ActivePayerStatus">,
  read Top15Percent: EnumItem<"ActivePayerStatus">,
}
type ActuatorRelativeTo = {
  read Attachment0: EnumItem<"ActuatorRelativeTo">,
  read Attachment1: EnumItem<"ActuatorRelativeTo">,
  read World: EnumItem<"ActuatorRelativeTo">,
}
type ActuatorType = {
  read None: EnumItem<"ActuatorType">,
  read Motor: EnumItem<"ActuatorType">,
  read Servo: EnumItem<"ActuatorType">,
}
type AdAvailabilityResult = {
  read IsAvailable: EnumItem<"AdAvailabilityResult">,
  read DeviceIneligible: EnumItem<"AdAvailabilityResult">,
  read ExperienceIneligible: EnumItem<"AdAvailabilityResult">,
  read InternalError: EnumItem<"AdAvailabilityResult">,
  read NoFill: EnumItem<"AdAvailabilityResult">,
  read PlayerIneligible: EnumItem<"AdAvailabilityResult">,
  read PublisherIneligible: EnumItem<"AdAvailabilityResult">,
}
type AdEventType = {
  read VideoLoaded: EnumItem<"AdEventType">,
  read VideoRemoved: EnumItem<"AdEventType">,
  read UserCompletedVideo: EnumItem<"AdEventType">,
  read RewardedAdLoaded: EnumItem<"AdEventType">,
  read RewardedAdGrant: EnumItem<"AdEventType">,
  read RewardedAdUnloaded: EnumItem<"AdEventType">,
}
type AdFormat = {
  read RewardedVideo: EnumItem<"AdFormat">,
}
type AdShape = {
  read HorizontalRectangle: EnumItem<"AdShape">,
}
type AdTeleportMethod = {
  read Undefined: EnumItem<"AdTeleportMethod">,
  read PortalForward: EnumItem<"AdTeleportMethod">,
  read InGameMenuBackButton: EnumItem<"AdTeleportMethod">,
  read UIBackButton: EnumItem<"AdTeleportMethod">,
}
type AdUIEventType = {
  read AdLabelClicked: EnumItem<"AdUIEventType">,
  read VolumeButtonClicked: EnumItem<"AdUIEventType">,
  read FullscreenButtonClicked: EnumItem<"AdUIEventType">,
  read PlayButtonClicked: EnumItem<"AdUIEventType">,
  read PauseButtonClicked: EnumItem<"AdUIEventType">,
  read CloseButtonClicked: EnumItem<"AdUIEventType">,
  read WhyThisAdClicked: EnumItem<"AdUIEventType">,
  read PlayEventTriggered: EnumItem<"AdUIEventType">,
  read PauseEventTriggered: EnumItem<"AdUIEventType">,
}
type AdUIType = {
  read None: EnumItem<"AdUIType">,
  read Image: EnumItem<"AdUIType">,
  read Video: EnumItem<"AdUIType">,
}
type AdUnitStatus = {
  read Inactive: EnumItem<"AdUnitStatus">,
  read Active: EnumItem<"AdUnitStatus">,
}
type AdornCullingMode = {
  read Automatic: EnumItem<"AdornCullingMode">,
  read Never: EnumItem<"AdornCullingMode">,
}
type AdornShading = {
  read Default: EnumItem<"AdornShading">,
  read Shaded: EnumItem<"AdornShading">,
  read XRay: EnumItem<"AdornShading">,
  read XRayShaded: EnumItem<"AdornShading">,
  read AlwaysOnTop: EnumItem<"AdornShading">,
}
type AgeCheckStatus = {
  read Unchecked: EnumItem<"AgeCheckStatus">,
  read Checked: EnumItem<"AgeCheckStatus">,
}
type AlignType = {
  read Parallel: EnumItem<"AlignType">,
  read Perpendicular: EnumItem<"AlignType">,
  read PrimaryAxisParallel: EnumItem<"AlignType">,
  read PrimaryAxisPerpendicular: EnumItem<"AlignType">,
  read PrimaryAxisLookAt: EnumItem<"AlignType">,
  read AllAxes: EnumItem<"AlignType">,
}
type AlphaMode = {
  read Overlay: EnumItem<"AlphaMode">,
  read Transparency: EnumItem<"AlphaMode">,
  read TintMask: EnumItem<"AlphaMode">,
  read Opaque: EnumItem<"AlphaMode">,
}
type AnalyticsCustomFieldKeys = {
  read CustomField01: EnumItem<"AnalyticsCustomFieldKeys">,
  read CustomField02: EnumItem<"AnalyticsCustomFieldKeys">,
  read CustomField03: EnumItem<"AnalyticsCustomFieldKeys">,
}
type AnalyticsEconomyAction = {
  read Default: EnumItem<"AnalyticsEconomyAction">,
  read Acquire: EnumItem<"AnalyticsEconomyAction">,
  read Spend: EnumItem<"AnalyticsEconomyAction">,
}
type AnalyticsEconomyFlowType = {
  read Sink: EnumItem<"AnalyticsEconomyFlowType">,
  read Source: EnumItem<"AnalyticsEconomyFlowType">,
}
type AnalyticsEconomyTransactionType = {
  read IAP: EnumItem<"AnalyticsEconomyTransactionType">,
  read Shop: EnumItem<"AnalyticsEconomyTransactionType">,
  read Gameplay: EnumItem<"AnalyticsEconomyTransactionType">,
  read ContextualPurchase: EnumItem<"AnalyticsEconomyTransactionType">,
  read TimedReward: EnumItem<"AnalyticsEconomyTransactionType">,
  read Onboarding: EnumItem<"AnalyticsEconomyTransactionType">,
}
type AnalyticsLogLevel = {
  read Trace: EnumItem<"AnalyticsLogLevel">,
  read Debug: EnumItem<"AnalyticsLogLevel">,
  read Information: EnumItem<"AnalyticsLogLevel">,
  read Warning: EnumItem<"AnalyticsLogLevel">,
  read Error: EnumItem<"AnalyticsLogLevel">,
  read Fatal: EnumItem<"AnalyticsLogLevel">,
}
type AnalyticsProgressionStatus = {
  read Default: EnumItem<"AnalyticsProgressionStatus">,
  read Begin: EnumItem<"AnalyticsProgressionStatus">,
  read Complete: EnumItem<"AnalyticsProgressionStatus">,
  read Abandon: EnumItem<"AnalyticsProgressionStatus">,
  read Fail: EnumItem<"AnalyticsProgressionStatus">,
}
type AnalyticsProgressionType = {
  read Custom: EnumItem<"AnalyticsProgressionType">,
  read Start: EnumItem<"AnalyticsProgressionType">,
  read Fail: EnumItem<"AnalyticsProgressionType">,
  read Complete: EnumItem<"AnalyticsProgressionType">,
}
type AnimationClipFromVideoStatus = {
  read Initializing: EnumItem<"AnimationClipFromVideoStatus">,
  read Pending: EnumItem<"AnimationClipFromVideoStatus">,
  read Processing: EnumItem<"AnimationClipFromVideoStatus">,
  read ErrorGeneric: EnumItem<"AnimationClipFromVideoStatus">,
  read Success: EnumItem<"AnimationClipFromVideoStatus">,
  read ErrorVideoTooLong: EnumItem<"AnimationClipFromVideoStatus">,
  read ErrorNoPersonDetected: EnumItem<"AnimationClipFromVideoStatus">,
  read ErrorVideoUnstable: EnumItem<"AnimationClipFromVideoStatus">,
  read Timeout: EnumItem<"AnimationClipFromVideoStatus">,
  read Cancelled: EnumItem<"AnimationClipFromVideoStatus">,
  read ErrorMultiplePeople: EnumItem<"AnimationClipFromVideoStatus">,
  read ErrorUploadingVideo: EnumItem<"AnimationClipFromVideoStatus">,
}
type AnimationNodeBlend2DInputMode = {
  read Cartesian: EnumItem<"AnimationNodeBlend2DInputMode">,
  read Polar: EnumItem<"AnimationNodeBlend2DInputMode">,
}
type AnimationNodeInterruptible = {
  read Always: EnumItem<"AnimationNodeInterruptible">,
  read Finished: EnumItem<"AnimationNodeInterruptible">,
  read Trigger: EnumItem<"AnimationNodeInterruptible">,
}
type AnimationNodePhaseSync = {
  read Synced: EnumItem<"AnimationNodePhaseSync">,
  read Unsynced: EnumItem<"AnimationNodePhaseSync">,
}
type AnimationNodePlayMode = {
  read Loop: EnumItem<"AnimationNodePlayMode">,
  read PingPong: EnumItem<"AnimationNodePlayMode">,
  read OnceAndHold: EnumItem<"AnimationNodePlayMode">,
  read OnceAndReset: EnumItem<"AnimationNodePlayMode">,
}
type AnimationNodeTransitionType = {
  read CrossFade: EnumItem<"AnimationNodeTransitionType">,
  read InertialBlend: EnumItem<"AnimationNodeTransitionType">,
  read DeadBlend: EnumItem<"AnimationNodeTransitionType">,
}
type AnimationNodeType = {
  read InvalidNode: EnumItem<"AnimationNodeType">,
  read AddNode: EnumItem<"AnimationNodeType">,
  read OverNode: EnumItem<"AnimationNodeType">,
  read Blend1DNode: EnumItem<"AnimationNodeType">,
  read Blend2DNode: EnumItem<"AnimationNodeType">,
  read ClipNode: EnumItem<"AnimationNodeType">,
  read GraphOutput: EnumItem<"AnimationNodeType">,
  read MaskNode: EnumItem<"AnimationNodeType">,
  read PrioritySelectNode: EnumItem<"AnimationNodeType">,
  read RandomSequenceNode: EnumItem<"AnimationNodeType">,
  read SelectNode: EnumItem<"AnimationNodeType">,
  read SequenceNode: EnumItem<"AnimationNodeType">,
  read SpeedNode: EnumItem<"AnimationNodeType">,
  read SubtractNode: EnumItem<"AnimationNodeType">,
}
type AnimationNodeWaitFor = {
  read Finished: EnumItem<"AnimationNodeWaitFor">,
  read Trigger: EnumItem<"AnimationNodeWaitFor">,
}
type AnimationPriority = {
  read Idle: EnumItem<"AnimationPriority">,
  read Movement: EnumItem<"AnimationPriority">,
  read Action: EnumItem<"AnimationPriority">,
  read Action2: EnumItem<"AnimationPriority">,
  read Action3: EnumItem<"AnimationPriority">,
  read Action4: EnumItem<"AnimationPriority">,
  read Core: EnumItem<"AnimationPriority">,
}
type AnimatorRetargetingMode = {
  read Default: EnumItem<"AnimatorRetargetingMode">,
  read Disabled: EnumItem<"AnimatorRetargetingMode">,
  read Enabled: EnumItem<"AnimatorRetargetingMode">,
}
type AnnotationChannelContentPreference = {
  read None: EnumItem<"AnnotationChannelContentPreference">,
  read All: EnumItem<"AnnotationChannelContentPreference">,
  read Unknown: EnumItem<"AnnotationChannelContentPreference">,
}
type AnnotationEditingMode = {
  read None: EnumItem<"AnnotationEditingMode">,
  read PlacingNew: EnumItem<"AnnotationEditingMode">,
  read WritingNew: EnumItem<"AnnotationEditingMode">,
}
type AnnotationPlaceContentPreference = {
  read None: EnumItem<"AnnotationPlaceContentPreference">,
  read All: EnumItem<"AnnotationPlaceContentPreference">,
  read MentionsAndReplies: EnumItem<"AnnotationPlaceContentPreference">,
  read Unknown: EnumItem<"AnnotationPlaceContentPreference">,
}
type AnnotationRequestStatus = {
  read Success: EnumItem<"AnnotationRequestStatus">,
  read Loading: EnumItem<"AnnotationRequestStatus">,
  read ErrorInternalFailure: EnumItem<"AnnotationRequestStatus">,
  read ErrorNotFound: EnumItem<"AnnotationRequestStatus">,
  read ErrorModerated: EnumItem<"AnnotationRequestStatus">,
}
type AnnotationRequestType = {
  read Unknown: EnumItem<"AnnotationRequestType">,
  read Create: EnumItem<"AnnotationRequestType">,
  read Resolve: EnumItem<"AnnotationRequestType">,
  read Delete: EnumItem<"AnnotationRequestType">,
  read Edit: EnumItem<"AnnotationRequestType">,
}
type AntiAliasing = {
  read Disabled: EnumItem<"AntiAliasing">,
  read Enabled: EnumItem<"AntiAliasing">,
}
type AppLifecycleManagerState = {
  read Detached: EnumItem<"AppLifecycleManagerState">,
  read Active: EnumItem<"AppLifecycleManagerState">,
  read Inactive: EnumItem<"AppLifecycleManagerState">,
  read Hidden: EnumItem<"AppLifecycleManagerState">,
}
type AppShellActionType = {
  read None: EnumItem<"AppShellActionType">,
  read OpenApp: EnumItem<"AppShellActionType">,
  read TapChatTab: EnumItem<"AppShellActionType">,
  read TapConversationEntry: EnumItem<"AppShellActionType">,
  read TapAvatarTab: EnumItem<"AppShellActionType">,
  read ReadConversation: EnumItem<"AppShellActionType">,
  read TapGamePageTab: EnumItem<"AppShellActionType">,
  read TapHomePageTab: EnumItem<"AppShellActionType">,
  read GamePageLoaded: EnumItem<"AppShellActionType">,
  read HomePageLoaded: EnumItem<"AppShellActionType">,
  read AvatarEditorPageLoaded: EnumItem<"AppShellActionType">,
  read HomePageInteractive: EnumItem<"AppShellActionType">,
}
type AppShellFeature = {
  read None: EnumItem<"AppShellFeature">,
  read Chat: EnumItem<"AppShellFeature">,
  read AvatarEditor: EnumItem<"AppShellFeature">,
  read GamePage: EnumItem<"AppShellFeature">,
  read HomePage: EnumItem<"AppShellFeature">,
  read More: EnumItem<"AppShellFeature">,
  read Landing: EnumItem<"AppShellFeature">,
  read WatchPage: EnumItem<"AppShellFeature">,
}
type AppUpdateStatus = {
  read Unknown: EnumItem<"AppUpdateStatus">,
  read NotSupported: EnumItem<"AppUpdateStatus">,
  read Failed: EnumItem<"AppUpdateStatus">,
  read NotAvailable: EnumItem<"AppUpdateStatus">,
  read Available: EnumItem<"AppUpdateStatus">,
  read AvailableBoundChannel: EnumItem<"AppUpdateStatus">,
  read AvailableBetaProgram: EnumItem<"AppUpdateStatus">,
}
type ApplyStrokeMode = {
  read Contextual: EnumItem<"ApplyStrokeMode">,
  read Border: EnumItem<"ApplyStrokeMode">,
}
type AspectType = {
  read FitWithinMaxSize: EnumItem<"AspectType">,
  read ScaleWithParentSize: EnumItem<"AspectType">,
}
type AssetCreatorType = {
  read User: EnumItem<"AssetCreatorType">,
  read Group: EnumItem<"AssetCreatorType">,
}
type AssetFetchStatus = {
  read Success: EnumItem<"AssetFetchStatus">,
  read Failure: EnumItem<"AssetFetchStatus">,
  read None: EnumItem<"AssetFetchStatus">,
  read Loading: EnumItem<"AssetFetchStatus">,
  read TimedOut: EnumItem<"AssetFetchStatus">,
}
type AssetRepresentation = {
  read FullLength: EnumItem<"AssetRepresentation">,
  read ShortPreview: EnumItem<"AssetRepresentation">,
}
type AssetType = {
  read Image: EnumItem<"AssetType">,
  read TShirt: EnumItem<"AssetType">,
  read Audio: EnumItem<"AssetType">,
  read Mesh: EnumItem<"AssetType">,
  read Lua: EnumItem<"AssetType">,
  read Hat: EnumItem<"AssetType">,
  read Place: EnumItem<"AssetType">,
  read Model: EnumItem<"AssetType">,
  read Shirt: EnumItem<"AssetType">,
  read Pants: EnumItem<"AssetType">,
  read Decal: EnumItem<"AssetType">,
  read Head: EnumItem<"AssetType">,
  read Face: EnumItem<"AssetType">,
  read Gear: EnumItem<"AssetType">,
  read Badge: EnumItem<"AssetType">,
  read Animation: EnumItem<"AssetType">,
  read Torso: EnumItem<"AssetType">,
  read RightArm: EnumItem<"AssetType">,
  read LeftArm: EnumItem<"AssetType">,
  read LeftLeg: EnumItem<"AssetType">,
  read RightLeg: EnumItem<"AssetType">,
  read Package: EnumItem<"AssetType">,
  read GamePass: EnumItem<"AssetType">,
  read Plugin: EnumItem<"AssetType">,
  read MeshPart: EnumItem<"AssetType">,
  read HairAccessory: EnumItem<"AssetType">,
  read FaceAccessory: EnumItem<"AssetType">,
  read NeckAccessory: EnumItem<"AssetType">,
  read ShoulderAccessory: EnumItem<"AssetType">,
  read FrontAccessory: EnumItem<"AssetType">,
  read BackAccessory: EnumItem<"AssetType">,
  read WaistAccessory: EnumItem<"AssetType">,
  read ClimbAnimation: EnumItem<"AssetType">,
  read DeathAnimation: EnumItem<"AssetType">,
  read FallAnimation: EnumItem<"AssetType">,
  read IdleAnimation: EnumItem<"AssetType">,
  read JumpAnimation: EnumItem<"AssetType">,
  read RunAnimation: EnumItem<"AssetType">,
  read SwimAnimation: EnumItem<"AssetType">,
  read WalkAnimation: EnumItem<"AssetType">,
  read PoseAnimation: EnumItem<"AssetType">,
  read EarAccessory: EnumItem<"AssetType">,
  read EyeAccessory: EnumItem<"AssetType">,
  read EmoteAnimation: EnumItem<"AssetType">,
  read Video: EnumItem<"AssetType">,
  read TShirtAccessory: EnumItem<"AssetType">,
  read ShirtAccessory: EnumItem<"AssetType">,
  read PantsAccessory: EnumItem<"AssetType">,
  read JacketAccessory: EnumItem<"AssetType">,
  read SweaterAccessory: EnumItem<"AssetType">,
  read ShortsAccessory: EnumItem<"AssetType">,
  read LeftShoeAccessory: EnumItem<"AssetType">,
  read RightShoeAccessory: EnumItem<"AssetType">,
  read DressSkirtAccessory: EnumItem<"AssetType">,
  read FontFamily: EnumItem<"AssetType">,
  read EyebrowAccessory: EnumItem<"AssetType">,
  read EyelashAccessory: EnumItem<"AssetType">,
  read MoodAnimation: EnumItem<"AssetType">,
  read DynamicHead: EnumItem<"AssetType">,
  read FaceMakeup: EnumItem<"AssetType">,
  read LipMakeup: EnumItem<"AssetType">,
  read EyeMakeup: EnumItem<"AssetType">,
  read VoxelFragment: EnumItem<"AssetType">,
  read AvatarBackground: EnumItem<"AssetType">,
  read TextDocument: EnumItem<"AssetType">,
}
type AssetTypeVerification = {
  read Default: EnumItem<"AssetTypeVerification">,
  read ClientOnly: EnumItem<"AssetTypeVerification">,
  read Always: EnumItem<"AssetTypeVerification">,
}
type AudioApiRollout = {
  read Disabled: EnumItem<"AudioApiRollout">,
  read Automatic: EnumItem<"AudioApiRollout">,
  read Enabled: EnumItem<"AudioApiRollout">,
}
type AudioCaptureMode = {}
type AudioChannelLayout = {
  read Mono: EnumItem<"AudioChannelLayout">,
  read Stereo: EnumItem<"AudioChannelLayout">,
  read Quad: EnumItem<"AudioChannelLayout">,
  read Surround_5: EnumItem<"AudioChannelLayout">,
  read Surround_5_1: EnumItem<"AudioChannelLayout">,
  read Surround_7_1: EnumItem<"AudioChannelLayout">,
  read Surround_7_1_4: EnumItem<"AudioChannelLayout">,
}
type AudioFilterType = {
  read Peak: EnumItem<"AudioFilterType">,
  read LowShelf: EnumItem<"AudioFilterType">,
  read HighShelf: EnumItem<"AudioFilterType">,
  read Lowpass12dB: EnumItem<"AudioFilterType">,
  read Lowpass24dB: EnumItem<"AudioFilterType">,
  read Lowpass48dB: EnumItem<"AudioFilterType">,
  read Highpass12dB: EnumItem<"AudioFilterType">,
  read Highpass24dB: EnumItem<"AudioFilterType">,
  read Highpass48dB: EnumItem<"AudioFilterType">,
  read Bandpass: EnumItem<"AudioFilterType">,
  read Notch: EnumItem<"AudioFilterType">,
  read Lowpass6dB: EnumItem<"AudioFilterType">,
}
type AudioSimulationFidelity = {
  read None: EnumItem<"AudioSimulationFidelity">,
  read Automatic: EnumItem<"AudioSimulationFidelity">,
}
type AudioSubType = {
  read Music: EnumItem<"AudioSubType">,
  read SoundEffect: EnumItem<"AudioSubType">,
}
type AudioWindowSize = {
  read Small: EnumItem<"AudioWindowSize">,
  read Medium: EnumItem<"AudioWindowSize">,
  read Large: EnumItem<"AudioWindowSize">,
}
type AuthorityMode = {
  read Server: EnumItem<"AuthorityMode">,
  read Automatic: EnumItem<"AuthorityMode">,
}
type AutoIndentRule = {
  read Off: EnumItem<"AutoIndentRule">,
  read Absolute: EnumItem<"AutoIndentRule">,
  read Relative: EnumItem<"AutoIndentRule">,
}
type AutomaticSize = {
  read None: EnumItem<"AutomaticSize">,
  read X: EnumItem<"AutomaticSize">,
  read Y: EnumItem<"AutomaticSize">,
  read XY: EnumItem<"AutomaticSize">,
}
type AvatarAssetType = {
  read TShirt: EnumItem<"AvatarAssetType">,
  read Hat: EnumItem<"AvatarAssetType">,
  read Shirt: EnumItem<"AvatarAssetType">,
  read Pants: EnumItem<"AvatarAssetType">,
  read Head: EnumItem<"AvatarAssetType">,
  read Face: EnumItem<"AvatarAssetType">,
  read Gear: EnumItem<"AvatarAssetType">,
  read Torso: EnumItem<"AvatarAssetType">,
  read RightArm: EnumItem<"AvatarAssetType">,
  read LeftArm: EnumItem<"AvatarAssetType">,
  read LeftLeg: EnumItem<"AvatarAssetType">,
  read RightLeg: EnumItem<"AvatarAssetType">,
  read HairAccessory: EnumItem<"AvatarAssetType">,
  read FaceAccessory: EnumItem<"AvatarAssetType">,
  read NeckAccessory: EnumItem<"AvatarAssetType">,
  read ShoulderAccessory: EnumItem<"AvatarAssetType">,
  read FrontAccessory: EnumItem<"AvatarAssetType">,
  read BackAccessory: EnumItem<"AvatarAssetType">,
  read WaistAccessory: EnumItem<"AvatarAssetType">,
  read ClimbAnimation: EnumItem<"AvatarAssetType">,
  read FallAnimation: EnumItem<"AvatarAssetType">,
  read IdleAnimation: EnumItem<"AvatarAssetType">,
  read JumpAnimation: EnumItem<"AvatarAssetType">,
  read RunAnimation: EnumItem<"AvatarAssetType">,
  read SwimAnimation: EnumItem<"AvatarAssetType">,
  read WalkAnimation: EnumItem<"AvatarAssetType">,
  read EmoteAnimation: EnumItem<"AvatarAssetType">,
  read TShirtAccessory: EnumItem<"AvatarAssetType">,
  read ShirtAccessory: EnumItem<"AvatarAssetType">,
  read PantsAccessory: EnumItem<"AvatarAssetType">,
  read JacketAccessory: EnumItem<"AvatarAssetType">,
  read SweaterAccessory: EnumItem<"AvatarAssetType">,
  read ShortsAccessory: EnumItem<"AvatarAssetType">,
  read LeftShoeAccessory: EnumItem<"AvatarAssetType">,
  read RightShoeAccessory: EnumItem<"AvatarAssetType">,
  read DressSkirtAccessory: EnumItem<"AvatarAssetType">,
  read EyebrowAccessory: EnumItem<"AvatarAssetType">,
  read EyelashAccessory: EnumItem<"AvatarAssetType">,
  read MoodAnimation: EnumItem<"AvatarAssetType">,
  read DynamicHead: EnumItem<"AvatarAssetType">,
  read FaceMakeup: EnumItem<"AvatarAssetType">,
  read LipMakeup: EnumItem<"AvatarAssetType">,
  read EyeMakeup: EnumItem<"AvatarAssetType">,
  read AvatarBackground: EnumItem<"AvatarAssetType">,
}
type AvatarChatServiceFeature = {
  read None: EnumItem<"AvatarChatServiceFeature">,
  read UniverseAudio: EnumItem<"AvatarChatServiceFeature">,
  read UniverseVideo: EnumItem<"AvatarChatServiceFeature">,
  read PlaceAudio: EnumItem<"AvatarChatServiceFeature">,
  read PlaceVideo: EnumItem<"AvatarChatServiceFeature">,
  read UserAudioEligible: EnumItem<"AvatarChatServiceFeature">,
  read UserAudio: EnumItem<"AvatarChatServiceFeature">,
  read UserVideoEligible: EnumItem<"AvatarChatServiceFeature">,
  read UserVideo: EnumItem<"AvatarChatServiceFeature">,
  read UserBanned: EnumItem<"AvatarChatServiceFeature">,
  read UserVerifiedForVoice: EnumItem<"AvatarChatServiceFeature">,
}
type AvatarContextMenuOption = {
  read Friend: EnumItem<"AvatarContextMenuOption">,
  read Chat: EnumItem<"AvatarContextMenuOption">,
  read Emote: EnumItem<"AvatarContextMenuOption">,
  read InspectMenu: EnumItem<"AvatarContextMenuOption">,
}
type AvatarGenerationError = {
  read None: EnumItem<"AvatarGenerationError">,
  read Unknown: EnumItem<"AvatarGenerationError">,
  read DownloadFailed: EnumItem<"AvatarGenerationError">,
  read Canceled: EnumItem<"AvatarGenerationError">,
  read Offensive: EnumItem<"AvatarGenerationError">,
  read Timeout: EnumItem<"AvatarGenerationError">,
  read JobNotFound: EnumItem<"AvatarGenerationError">,
}
type AvatarItemType = {
  read Asset: EnumItem<"AvatarItemType">,
  read Bundle: EnumItem<"AvatarItemType">,
}
type AvatarPromptResult = {
  read Success: EnumItem<"AvatarPromptResult">,
  read PermissionDenied: EnumItem<"AvatarPromptResult">,
  read Failed: EnumItem<"AvatarPromptResult">,
}
type AvatarSettingsAccessoryLimitMethod = {
  read Scale: EnumItem<"AvatarSettingsAccessoryLimitMethod">,
  read Remove: EnumItem<"AvatarSettingsAccessoryLimitMethod">,
  read PreviewScale: EnumItem<"AvatarSettingsAccessoryLimitMethod">,
  read PreviewRemove: EnumItem<"AvatarSettingsAccessoryLimitMethod">,
}
type AvatarSettingsAccessoryMode = {
  read PlayerChoice: EnumItem<"AvatarSettingsAccessoryMode">,
  read CustomLimit: EnumItem<"AvatarSettingsAccessoryMode">,
}
type AvatarSettingsAnimationClipsMode = {
  read PlayerChoice: EnumItem<"AvatarSettingsAnimationClipsMode">,
  read CustomClips: EnumItem<"AvatarSettingsAnimationClipsMode">,
}
type AvatarSettingsAnimationPacksMode = {
  read PlayerChoice: EnumItem<"AvatarSettingsAnimationPacksMode">,
  read StandardR15: EnumItem<"AvatarSettingsAnimationPacksMode">,
  read StandardR6: EnumItem<"AvatarSettingsAnimationPacksMode">,
}
type AvatarSettingsAppearanceMode = {
  read PlayerChoice: EnumItem<"AvatarSettingsAppearanceMode">,
  read CustomParts: EnumItem<"AvatarSettingsAppearanceMode">,
  read CustomBody: EnumItem<"AvatarSettingsAppearanceMode">,
}
type AvatarSettingsBuildMode = {
  read PlayerChoice: EnumItem<"AvatarSettingsBuildMode">,
  read CustomBuild: EnumItem<"AvatarSettingsBuildMode">,
}
type AvatarSettingsCharacterControllerMode = {
  read LegacyHumanoid: EnumItem<"AvatarSettingsCharacterControllerMode">,
  read LuaCharacterController: EnumItem<"AvatarSettingsCharacterControllerMode">,
}
type AvatarSettingsClothingMode = {
  read PlayerChoice: EnumItem<"AvatarSettingsClothingMode">,
  read CustomLimit: EnumItem<"AvatarSettingsClothingMode">,
}
type AvatarSettingsCollisionMode = {
  read Default: EnumItem<"AvatarSettingsCollisionMode">,
  read SingleCollider: EnumItem<"AvatarSettingsCollisionMode">,
  read Legacy: EnumItem<"AvatarSettingsCollisionMode">,
}
type AvatarSettingsCustomAccessoryMode = {
  read PlayerChoice: EnumItem<"AvatarSettingsCustomAccessoryMode">,
  read CustomAccessories: EnumItem<"AvatarSettingsCustomAccessoryMode">,
}
type AvatarSettingsCustomBodyType = {
  read AvatarReference: EnumItem<"AvatarSettingsCustomBodyType">,
  read BundleId: EnumItem<"AvatarSettingsCustomBodyType">,
}
type AvatarSettingsCustomClothingMode = {
  read PlayerChoice: EnumItem<"AvatarSettingsCustomClothingMode">,
  read CustomClothing: EnumItem<"AvatarSettingsCustomClothingMode">,
}
type AvatarSettingsHitAndTouchDetectionMode = {
  read UseParts: EnumItem<"AvatarSettingsHitAndTouchDetectionMode">,
  read UseCollider: EnumItem<"AvatarSettingsHitAndTouchDetectionMode">,
}
type AvatarSettingsJumpMode = {
  read JumpHeight: EnumItem<"AvatarSettingsJumpMode">,
  read JumpPower: EnumItem<"AvatarSettingsJumpMode">,
}
type AvatarSettingsLegacyCollisionMode = {
  read R6Colliders: EnumItem<"AvatarSettingsLegacyCollisionMode">,
  read InnerBoxColliders: EnumItem<"AvatarSettingsLegacyCollisionMode">,
}
type AvatarSettingsScaleMode = {
  read PlayerChoice: EnumItem<"AvatarSettingsScaleMode">,
  read CustomScale: EnumItem<"AvatarSettingsScaleMode">,
}
type AvatarThumbnailCustomizationType = {
  read Closeup: EnumItem<"AvatarThumbnailCustomizationType">,
  read FullBody: EnumItem<"AvatarThumbnailCustomizationType">,
}
type AvatarUnificationMode = {
  read Default: EnumItem<"AvatarUnificationMode">,
  read Disabled: EnumItem<"AvatarUnificationMode">,
  read Enabled: EnumItem<"AvatarUnificationMode">,
}
type Axis = {
  read X: EnumItem<"Axis">,
  read Y: EnumItem<"Axis">,
  read Z: EnumItem<"Axis">,
}
type BenefitType = {
  read DeveloperProduct: EnumItem<"BenefitType">,
  read AvatarAsset: EnumItem<"BenefitType">,
  read AvatarBundle: EnumItem<"BenefitType">,
}
type BinType = {
  read Script: EnumItem<"BinType">,
  read GameTool: EnumItem<"BinType">,
  read Grab: EnumItem<"BinType">,
  read Clone: EnumItem<"BinType">,
  read Hammer: EnumItem<"BinType">,
}
type BodyPart = {
  read Head: EnumItem<"BodyPart">,
  read Torso: EnumItem<"BodyPart">,
  read LeftArm: EnumItem<"BodyPart">,
  read RightArm: EnumItem<"BodyPart">,
  read LeftLeg: EnumItem<"BodyPart">,
  read RightLeg: EnumItem<"BodyPart">,
}
type BodyPartR15 = {
  read Head: EnumItem<"BodyPartR15">,
  read UpperTorso: EnumItem<"BodyPartR15">,
  read LowerTorso: EnumItem<"BodyPartR15">,
  read LeftFoot: EnumItem<"BodyPartR15">,
  read LeftLowerLeg: EnumItem<"BodyPartR15">,
  read LeftUpperLeg: EnumItem<"BodyPartR15">,
  read RightFoot: EnumItem<"BodyPartR15">,
  read RightLowerLeg: EnumItem<"BodyPartR15">,
  read RightUpperLeg: EnumItem<"BodyPartR15">,
  read LeftHand: EnumItem<"BodyPartR15">,
  read LeftLowerArm: EnumItem<"BodyPartR15">,
  read LeftUpperArm: EnumItem<"BodyPartR15">,
  read RightHand: EnumItem<"BodyPartR15">,
  read RightLowerArm: EnumItem<"BodyPartR15">,
  read RightUpperArm: EnumItem<"BodyPartR15">,
  read RootPart: EnumItem<"BodyPartR15">,
  read Unknown: EnumItem<"BodyPartR15">,
}
type BorderMode = {
  read Outline: EnumItem<"BorderMode">,
  read Middle: EnumItem<"BorderMode">,
  read Inset: EnumItem<"BorderMode">,
}
type BorderStrokePosition = {
  read Outer: EnumItem<"BorderStrokePosition">,
  read Center: EnumItem<"BorderStrokePosition">,
  read Inner: EnumItem<"BorderStrokePosition">,
}
type BreakReason = {
  read Other: EnumItem<"BreakReason">,
  read Error: EnumItem<"BreakReason">,
  read SpecialBreakpoint: EnumItem<"BreakReason">,
  read UserBreakpoint: EnumItem<"BreakReason">,
}
type BreakpointRemoveReason = {
  read Requested: EnumItem<"BreakpointRemoveReason">,
  read ScriptChanged: EnumItem<"BreakpointRemoveReason">,
  read ScriptRemoved: EnumItem<"BreakpointRemoveReason">,
}
type BulkMoveMode = {
  read FireAllEvents: EnumItem<"BulkMoveMode">,
  read FireCFrameChanged: EnumItem<"BulkMoveMode">,
}
type BundleType = {
  read BodyParts: EnumItem<"BundleType">,
  read Animations: EnumItem<"BundleType">,
  read Shoes: EnumItem<"BundleType">,
  read DynamicHead: EnumItem<"BundleType">,
  read DynamicHeadAvatar: EnumItem<"BundleType">,
}
type Button = {
  read Dismount: EnumItem<"Button">,
  read Jump: EnumItem<"Button">,
}
type ButtonStyle = {
  read Custom: EnumItem<"ButtonStyle">,
  read RobloxButtonDefault: EnumItem<"ButtonStyle">,
  read RobloxButton: EnumItem<"ButtonStyle">,
  read RobloxRoundButton: EnumItem<"ButtonStyle">,
  read RobloxRoundDefaultButton: EnumItem<"ButtonStyle">,
  read RobloxRoundDropdownButton: EnumItem<"ButtonStyle">,
}
type CageType = {
  read Inner: EnumItem<"CageType">,
  read Outer: EnumItem<"CageType">,
}
type CameraMode = {
  read Classic: EnumItem<"CameraMode">,
  read LockFirstPerson: EnumItem<"CameraMode">,
}
type CameraNavigationModel = {
  read Roblox: EnumItem<"CameraNavigationModel">,
  read IndustryCompatible: EnumItem<"CameraNavigationModel">,
}
type CameraPanMode = {
  read Classic: EnumItem<"CameraPanMode">,
  read EdgeBump: EnumItem<"CameraPanMode">,
}
type CameraSpeedAdjustBinding = {
  read None: EnumItem<"CameraSpeedAdjustBinding">,
  read RmbScroll: EnumItem<"CameraSpeedAdjustBinding">,
  read AltScroll: EnumItem<"CameraSpeedAdjustBinding">,
}
type CameraType = {
  read Fixed: EnumItem<"CameraType">,
  read Attach: EnumItem<"CameraType">,
  read Watch: EnumItem<"CameraType">,
  read Track: EnumItem<"CameraType">,
  read Follow: EnumItem<"CameraType">,
  read Custom: EnumItem<"CameraType">,
  read Scriptable: EnumItem<"CameraType">,
  read Orbital: EnumItem<"CameraType">,
}
type CanCollaborateError = {
  read Invalid: EnumItem<"CanCollaborateError">,
  read None: EnumItem<"CanCollaborateError">,
  read NotAgeVerified: EnumItem<"CanCollaborateError">,
  read OutsideAgeBucket: EnumItem<"CanCollaborateError">,
  read TooManyCollaborators: EnumItem<"CanCollaborateError">,
  read PCBlock: EnumItem<"CanCollaborateError">,
  read NotFound: EnumItem<"CanCollaborateError">,
  read OutsideOwnerAgeBucket: EnumItem<"CanCollaborateError">,
  read NotAuthorized: EnumItem<"CanCollaborateError">,
  read OutsideAgeBucketTcPc: EnumItem<"CanCollaborateError">,
  read OtherCollaboratorSettingsPreventTrust: EnumItem<"CanCollaborateError">,
  read AgeVerificationCountryBlocked: EnumItem<"CanCollaborateError">,
  read OtherUserCannotCollaborate: EnumItem<"CanCollaborateError">,
}
type CaptureGalleryPermission = {
  read ReadAndUpload: EnumItem<"CaptureGalleryPermission">,
}
type CaptureType = {
  read Screenshot: EnumItem<"CaptureType">,
  read Video: EnumItem<"CaptureType">,
}
type CatalogCategoryFilter = {
  read None: EnumItem<"CatalogCategoryFilter">,
  read Featured: EnumItem<"CatalogCategoryFilter">,
  read Collectibles: EnumItem<"CatalogCategoryFilter">,
  read CommunityCreations: EnumItem<"CatalogCategoryFilter">,
  read Premium: EnumItem<"CatalogCategoryFilter">,
  read Recommended: EnumItem<"CatalogCategoryFilter">,
}
type CatalogSortAggregation = {
  read Past12Hours: EnumItem<"CatalogSortAggregation">,
  read PastDay: EnumItem<"CatalogSortAggregation">,
  read Past3Days: EnumItem<"CatalogSortAggregation">,
  read PastWeek: EnumItem<"CatalogSortAggregation">,
  read PastMonth: EnumItem<"CatalogSortAggregation">,
  read AllTime: EnumItem<"CatalogSortAggregation">,
}
type CatalogSortType = {
  read Relevance: EnumItem<"CatalogSortType">,
  read PriceHighToLow: EnumItem<"CatalogSortType">,
  read PriceLowToHigh: EnumItem<"CatalogSortType">,
  read MostFavorited: EnumItem<"CatalogSortType">,
  read RecentlyCreated: EnumItem<"CatalogSortType">,
  read Bestselling: EnumItem<"CatalogSortType">,
}
type CellBlock = {
  read Solid: EnumItem<"CellBlock">,
  read VerticalWedge: EnumItem<"CellBlock">,
  read CornerWedge: EnumItem<"CellBlock">,
  read InverseCornerWedge: EnumItem<"CellBlock">,
  read HorizontalWedge: EnumItem<"CellBlock">,
}
type CellMaterial = {
  read Empty: EnumItem<"CellMaterial">,
  read Grass: EnumItem<"CellMaterial">,
  read Sand: EnumItem<"CellMaterial">,
  read Brick: EnumItem<"CellMaterial">,
  read Granite: EnumItem<"CellMaterial">,
  read Asphalt: EnumItem<"CellMaterial">,
  read Iron: EnumItem<"CellMaterial">,
  read Aluminum: EnumItem<"CellMaterial">,
  read Gold: EnumItem<"CellMaterial">,
  read WoodPlank: EnumItem<"CellMaterial">,
  read WoodLog: EnumItem<"CellMaterial">,
  read Gravel: EnumItem<"CellMaterial">,
  read CinderBlock: EnumItem<"CellMaterial">,
  read MossyStone: EnumItem<"CellMaterial">,
  read Cement: EnumItem<"CellMaterial">,
  read RedPlastic: EnumItem<"CellMaterial">,
  read BluePlastic: EnumItem<"CellMaterial">,
  read Water: EnumItem<"CellMaterial">,
}
type CellOrientation = {
  read NegZ: EnumItem<"CellOrientation">,
  read X: EnumItem<"CellOrientation">,
  read Z: EnumItem<"CellOrientation">,
  read NegX: EnumItem<"CellOrientation">,
}
type CenterDialogType = {
  read UnsolicitedDialog: EnumItem<"CenterDialogType">,
  read PlayerInitiatedDialog: EnumItem<"CenterDialogType">,
  read ModalDialog: EnumItem<"CenterDialogType">,
  read QuitDialog: EnumItem<"CenterDialogType">,
}
type CharacterControlMode = {
  read Default: EnumItem<"CharacterControlMode">,
  read Legacy: EnumItem<"CharacterControlMode">,
  read NoCharacterController: EnumItem<"CharacterControlMode">,
  read LuaCharacterController: EnumItem<"CharacterControlMode">,
}
type ChatCallbackType = {
  read OnCreatingChatWindow: EnumItem<"ChatCallbackType">,
  read OnClientSendingMessage: EnumItem<"ChatCallbackType">,
  read OnClientFormattingMessage: EnumItem<"ChatCallbackType">,
  read OnServerReceivingMessage: EnumItem<"ChatCallbackType">,
}
type ChatColor = {
  read Blue: EnumItem<"ChatColor">,
  read Green: EnumItem<"ChatColor">,
  read Red: EnumItem<"ChatColor">,
  read White: EnumItem<"ChatColor">,
}
type ChatMode = {
  read Menu: EnumItem<"ChatMode">,
  read TextAndMenu: EnumItem<"ChatMode">,
}
type ChatPrivacyMode = {
  read AllUsers: EnumItem<"ChatPrivacyMode">,
  read NoOne: EnumItem<"ChatPrivacyMode">,
  read Friends: EnumItem<"ChatPrivacyMode">,
}
type ChatRestrictionStatus = {
  read Unknown: EnumItem<"ChatRestrictionStatus">,
  read NotRestricted: EnumItem<"ChatRestrictionStatus">,
  read Restricted: EnumItem<"ChatRestrictionStatus">,
}
type ChatStyle = {
  read Classic: EnumItem<"ChatStyle">,
  read Bubble: EnumItem<"ChatStyle">,
  read ClassicAndBubble: EnumItem<"ChatStyle">,
}
type ChatVersion = {
  read LegacyChatService: EnumItem<"ChatVersion">,
  read TextChatService: EnumItem<"ChatVersion">,
}
type ClientAnimatorThrottlingMode = {
  read Default: EnumItem<"ClientAnimatorThrottlingMode">,
  read Disabled: EnumItem<"ClientAnimatorThrottlingMode">,
  read Enabled: EnumItem<"ClientAnimatorThrottlingMode">,
}
type CloseReason = {
  read Unknown: EnumItem<"CloseReason">,
  read RobloxMaintenance: EnumItem<"CloseReason">,
  read DeveloperShutdown: EnumItem<"CloseReason">,
  read DeveloperUpdate: EnumItem<"CloseReason">,
  read ServerEmpty: EnumItem<"CloseReason">,
  read OutOfMemory: EnumItem<"CloseReason">,
  read Moderation: EnumItem<"CloseReason">,
}
type CollaboratorStatus = {
  read None: EnumItem<"CollaboratorStatus">,
  read Editing3D: EnumItem<"CollaboratorStatus">,
  read Scripting: EnumItem<"CollaboratorStatus">,
  read PrivateScripting: EnumItem<"CollaboratorStatus">,
}
type CollisionFidelity = {
  read Default: EnumItem<"CollisionFidelity">,
  read Hull: EnumItem<"CollisionFidelity">,
  read Box: EnumItem<"CollisionFidelity">,
  read PreciseConvexDecomposition: EnumItem<"CollisionFidelity">,
  read Scalable: EnumItem<"CollisionFidelity">,
}
type CommandPermission = {
  read Plugin: EnumItem<"CommandPermission">,
  read LocalUser: EnumItem<"CommandPermission">,
}
type CompileTarget = {
  read Client: EnumItem<"CompileTarget">,
  read CoreScript: EnumItem<"CompileTarget">,
  read Studio: EnumItem<"CompileTarget">,
  read CoreScriptRaw: EnumItem<"CompileTarget">,
}
type CompletionAcceptanceBehavior = {
  read Insert: EnumItem<"CompletionAcceptanceBehavior">,
  read Replace: EnumItem<"CompletionAcceptanceBehavior">,
  read ReplaceOnEnterInsertOnTab: EnumItem<"CompletionAcceptanceBehavior">,
  read InsertOnEnterReplaceOnTab: EnumItem<"CompletionAcceptanceBehavior">,
}
type CompletionItemKind = {
  read Text: EnumItem<"CompletionItemKind">,
  read Method: EnumItem<"CompletionItemKind">,
  read Function: EnumItem<"CompletionItemKind">,
  read Constructor: EnumItem<"CompletionItemKind">,
  read Field: EnumItem<"CompletionItemKind">,
  read Variable: EnumItem<"CompletionItemKind">,
  read Class: EnumItem<"CompletionItemKind">,
  read Interface: EnumItem<"CompletionItemKind">,
  read Module: EnumItem<"CompletionItemKind">,
  read Property: EnumItem<"CompletionItemKind">,
  read Unit: EnumItem<"CompletionItemKind">,
  read Value: EnumItem<"CompletionItemKind">,
  read Enum: EnumItem<"CompletionItemKind">,
  read Keyword: EnumItem<"CompletionItemKind">,
  read Snippet: EnumItem<"CompletionItemKind">,
  read Color: EnumItem<"CompletionItemKind">,
  read File: EnumItem<"CompletionItemKind">,
  read Reference: EnumItem<"CompletionItemKind">,
  read Folder: EnumItem<"CompletionItemKind">,
  read EnumMember: EnumItem<"CompletionItemKind">,
  read Constant: EnumItem<"CompletionItemKind">,
  read Struct: EnumItem<"CompletionItemKind">,
  read Event: EnumItem<"CompletionItemKind">,
  read Operator: EnumItem<"CompletionItemKind">,
  read TypeParameter: EnumItem<"CompletionItemKind">,
}
type CompletionItemTag = {
  read Deprecated: EnumItem<"CompletionItemTag">,
  read IncorrectIndexType: EnumItem<"CompletionItemTag">,
  read PluginPermissions: EnumItem<"CompletionItemTag">,
  read CommandLinePermissions: EnumItem<"CompletionItemTag">,
  read RobloxPermissions: EnumItem<"CompletionItemTag">,
  read AddParens: EnumItem<"CompletionItemTag">,
  read PutCursorInParens: EnumItem<"CompletionItemTag">,
  read TypeCorrect: EnumItem<"CompletionItemTag">,
  read ClientServerBoundaryViolation: EnumItem<"CompletionItemTag">,
  read Invalidated: EnumItem<"CompletionItemTag">,
  read PutCursorBeforeEnd: EnumItem<"CompletionItemTag">,
}
type CompletionTriggerKind = {
  read Invoked: EnumItem<"CompletionTriggerKind">,
  read TriggerCharacter: EnumItem<"CompletionTriggerKind">,
  read TriggerForIncompleteCompletions: EnumItem<"CompletionTriggerKind">,
}
type CompositeValueCurveType = {
  read ColorRGB: EnumItem<"CompositeValueCurveType">,
  read ColorHSV: EnumItem<"CompositeValueCurveType">,
  read NumberRange: EnumItem<"CompositeValueCurveType">,
  read Rect: EnumItem<"CompositeValueCurveType">,
  read UDim: EnumItem<"CompositeValueCurveType">,
  read UDim2: EnumItem<"CompositeValueCurveType">,
  read Vector2: EnumItem<"CompositeValueCurveType">,
  read Vector3: EnumItem<"CompositeValueCurveType">,
}
type CompressionAlgorithm = {
  read Zstd: EnumItem<"CompressionAlgorithm">,
}
type ComputerCameraMovementMode = {
  read Default: EnumItem<"ComputerCameraMovementMode">,
  read Classic: EnumItem<"ComputerCameraMovementMode">,
  read Follow: EnumItem<"ComputerCameraMovementMode">,
  read Orbital: EnumItem<"ComputerCameraMovementMode">,
  read CameraToggle: EnumItem<"ComputerCameraMovementMode">,
}
type ComputerMovementMode = {
  read Default: EnumItem<"ComputerMovementMode">,
  read KeyboardMouse: EnumItem<"ComputerMovementMode">,
  read ClickToMove: EnumItem<"ComputerMovementMode">,
}
type ConfigSnapshotErrorState = {
  read None: EnumItem<"ConfigSnapshotErrorState">,
  read LoadFailed: EnumItem<"ConfigSnapshotErrorState">,
}
type ConnectionError = {
  read OK: EnumItem<"ConnectionError">,
  read Unknown: EnumItem<"ConnectionError">,
  read ConnectErrors: EnumItem<"ConnectionError">,
  read AlreadyConnected: EnumItem<"ConnectionError">,
  read NoFreeIncomingConnections: EnumItem<"ConnectionError">,
  read ConnectionBanned: EnumItem<"ConnectionError">,
  read InvalidPassword: EnumItem<"ConnectionError">,
  read IncompatibleProtocolVersion: EnumItem<"ConnectionError">,
  read IPRecentlyConnected: EnumItem<"ConnectionError">,
  read OurSystemRequiresSecurity: EnumItem<"ConnectionError">,
  read SecurityKeyMismatch: EnumItem<"ConnectionError">,
  read DisconnectErrors: EnumItem<"ConnectionError">,
  read DisconnectBadhash: EnumItem<"ConnectionError">,
  read DisconnectSecurityKeyMismatch: EnumItem<"ConnectionError">,
  read DisconnectProtocolMismatch: EnumItem<"ConnectionError">,
  read DisconnectReceivePacketError: EnumItem<"ConnectionError">,
  read DisconnectReceivePacketStreamError: EnumItem<"ConnectionError">,
  read DisconnectSendPacketError: EnumItem<"ConnectionError">,
  read DisconnectIllegalTeleport: EnumItem<"ConnectionError">,
  read DisconnectDuplicatePlayer: EnumItem<"ConnectionError">,
  read DisconnectDuplicateTicket: EnumItem<"ConnectionError">,
  read DisconnectTimeout: EnumItem<"ConnectionError">,
  read DisconnectLuaKick: EnumItem<"ConnectionError">,
  read DisconnectOnRemoteSysStats: EnumItem<"ConnectionError">,
  read DisconnectHashTimeout: EnumItem<"ConnectionError">,
  read DisconnectCloudEditKick: EnumItem<"ConnectionError">,
  read DisconnectPlayerless: EnumItem<"ConnectionError">,
  read DisconnectNewSecurityKeyMismatch: EnumItem<"ConnectionError">,
  read DisconnectEvicted: EnumItem<"ConnectionError">,
  read DisconnectDevMaintenance: EnumItem<"ConnectionError">,
  read DisconnectRobloxMaintenance: EnumItem<"ConnectionError">,
  read DisconnectRejoin: EnumItem<"ConnectionError">,
  read DisconnectConnectionLost: EnumItem<"ConnectionError">,
  read DisconnectIdle: EnumItem<"ConnectionError">,
  read DisconnectRaknetErrors: EnumItem<"ConnectionError">,
  read DisconnectWrongVersion: EnumItem<"ConnectionError">,
  read DisconnectBySecurityPolicy: EnumItem<"ConnectionError">,
  read DisconnectBlockedIP: EnumItem<"ConnectionError">,
  read DisconnectClientFailure: EnumItem<"ConnectionError">,
  read DisconnectClientRequest: EnumItem<"ConnectionError">,
  read DisconnectPrivateServerKickout: EnumItem<"ConnectionError">,
  read DisconnectModeratedGame: EnumItem<"ConnectionError">,
  read ServerShutdown: EnumItem<"ConnectionError">,
  read ReplicatorTimeout: EnumItem<"ConnectionError">,
  read PlayerRemoved: EnumItem<"ConnectionError">,
  read DisconnectOutOfMemoryKeepPlayingLeave: EnumItem<"ConnectionError">,
  read DisconnectRomarkEndOfTest: EnumItem<"ConnectionError">,
  read DisconnectCollaboratorPermissionRevoked: EnumItem<"ConnectionError">,
  read DisconnectCollaboratorUnderage: EnumItem<"ConnectionError">,
  read NetworkInternal: EnumItem<"ConnectionError">,
  read NetworkSend: EnumItem<"ConnectionError">,
  read NetworkTimeout: EnumItem<"ConnectionError">,
  read NetworkMisbehavior: EnumItem<"ConnectionError">,
  read NetworkSecurity: EnumItem<"ConnectionError">,
  read ReplacementReady: EnumItem<"ConnectionError">,
  read ServerEmpty: EnumItem<"ConnectionError">,
  read PhantomFreeze: EnumItem<"ConnectionError">,
  read AndroidAnticheatKick: EnumItem<"ConnectionError">,
  read AndroidEmulatorKick: EnumItem<"ConnectionError">,
  read AndroidRootedKick: EnumItem<"ConnectionError">,
  read ScreentimeLockoutKick: EnumItem<"ConnectionError">,
  read DisconnectionNotification: EnumItem<"ConnectionError">,
  read DisconnectVerboselyModeratedGame: EnumItem<"ConnectionError">,
  read DisconnectCollaboratorNotAgeVerified: EnumItem<"ConnectionError">,
  read DisconnectCollaboratorTrustedConnectionsRequired: EnumItem<"ConnectionError">,
  read DisconnectCollaboratorOwnerActionRequired: EnumItem<"ConnectionError">,
  read DisconnectCollaboratorTooManyCollaborators: EnumItem<"ConnectionError">,
  read DisconnectCollaboratorUnknownError: EnumItem<"ConnectionError">,
  read DisconnectCollaboratorRequestedEviction: EnumItem<"ConnectionError">,
  read DisconnectCollaboratorTrustedConnectionsRequiredPC: EnumItem<"ConnectionError">,
  read DisconnectRemoteAttestationUnsupported: EnumItem<"ConnectionError">,
  read DisconnectRemoteAttestationGeneralFailure: EnumItem<"ConnectionError">,
  read DisconnectRemoteAttestationTimeout: EnumItem<"ConnectionError">,
  read DisconnectRemoteAttestationOSOutOfDate: EnumItem<"ConnectionError">,
  read DisconnectRemoteAttestationBootValidationFailure: EnumItem<"ConnectionError">,
  read PlacelaunchErrors: EnumItem<"ConnectionError">,
  read PlacelaunchDisabled: EnumItem<"ConnectionError">,
  read PlacelaunchError: EnumItem<"ConnectionError">,
  read PlacelaunchGameEnded: EnumItem<"ConnectionError">,
  read PlacelaunchGameFull: EnumItem<"ConnectionError">,
  read PlacelaunchUserLeft: EnumItem<"ConnectionError">,
  read PlacelaunchRestricted: EnumItem<"ConnectionError">,
  read PlacelaunchUnauthorized: EnumItem<"ConnectionError">,
  read PlacelaunchFlooded: EnumItem<"ConnectionError">,
  read PlacelaunchHashExpired: EnumItem<"ConnectionError">,
  read PlacelaunchHashException: EnumItem<"ConnectionError">,
  read PlacelaunchPartyCannotFit: EnumItem<"ConnectionError">,
  read PlacelaunchHttpError: EnumItem<"ConnectionError">,
  read PlacelaunchUserPrivacyUnauthorized: EnumItem<"ConnectionError">,
  read PlacelaunchVipOwnerNotPresent: EnumItem<"ConnectionError">,
  read PlacelaunchAgeVerificationRequired: EnumItem<"ConnectionError">,
  read PlacelaunchParentalApprovalRequired: EnumItem<"ConnectionError">,
  read PlacelaunchCoreGated: EnumItem<"ConnectionError">,
  read PlacelaunchCollaborationCoreGated: EnumItem<"ConnectionError">,
  read PlacelaunchCreatorBan: EnumItem<"ConnectionError">,
  read PlacelaunchDeviceBlock: EnumItem<"ConnectionError">,
  read PlacelaunchCustomMessage: EnumItem<"ConnectionError">,
  read PlacelaunchOtherError: EnumItem<"ConnectionError">,
  read TeleportErrors: EnumItem<"ConnectionError">,
  read TeleportFailure: EnumItem<"ConnectionError">,
  read TeleportGameNotFound: EnumItem<"ConnectionError">,
  read TeleportGameEnded: EnumItem<"ConnectionError">,
  read TeleportGameFull: EnumItem<"ConnectionError">,
  read TeleportUnauthorized: EnumItem<"ConnectionError">,
  read TeleportFlooded: EnumItem<"ConnectionError">,
  read TeleportIsTeleporting: EnumItem<"ConnectionError">,
}
type ConnectionState = {
  read Connected: EnumItem<"ConnectionState">,
  read Disconnected: EnumItem<"ConnectionState">,
}
type ContentSourceType = {
  read None: EnumItem<"ContentSourceType">,
  read Uri: EnumItem<"ContentSourceType">,
  read Object: EnumItem<"ContentSourceType">,
  read Opaque: EnumItem<"ContentSourceType">,
}
type ContextActionPriority = {
  read Low: EnumItem<"ContextActionPriority">,
  read Medium: EnumItem<"ContextActionPriority">,
  read High: EnumItem<"ContextActionPriority">,
}
type ContextActionResult = {
  read Sink: EnumItem<"ContextActionResult">,
  read Pass: EnumItem<"ContextActionResult">,
}
type ControlMode = {
  read Classic: EnumItem<"ControlMode">,
  read MouseLockSwitch: EnumItem<"ControlMode">,
}
type CoreGuiType = {
  read PlayerList: EnumItem<"CoreGuiType">,
  read Health: EnumItem<"CoreGuiType">,
  read Backpack: EnumItem<"CoreGuiType">,
  read Chat: EnumItem<"CoreGuiType">,
  read All: EnumItem<"CoreGuiType">,
  read EmotesMenu: EnumItem<"CoreGuiType">,
  read SelfView: EnumItem<"CoreGuiType">,
  read Captures: EnumItem<"CoreGuiType">,
  read AvatarSwitcher: EnumItem<"CoreGuiType">,
  read ExperienceShop: EnumItem<"CoreGuiType">,
}
type CreateAssetResult = {
  read Success: EnumItem<"CreateAssetResult">,
  read PermissionDenied: EnumItem<"CreateAssetResult">,
  read UploadFailed: EnumItem<"CreateAssetResult">,
  read Unknown: EnumItem<"CreateAssetResult">,
}
type CreateContentResult = {
  read Success: EnumItem<"CreateContentResult">,
  read PermissionDenied: EnumItem<"CreateContentResult">,
  read UploadFailed: EnumItem<"CreateContentResult">,
  read StorageLimitExceeded: EnumItem<"CreateContentResult">,
  read Unknown: EnumItem<"CreateContentResult">,
}
type CreateOutfitFailure = {
  read InvalidName: EnumItem<"CreateOutfitFailure">,
  read OutfitLimitReached: EnumItem<"CreateOutfitFailure">,
  read Other: EnumItem<"CreateOutfitFailure">,
}
type CreatorType = {
  read User: EnumItem<"CreatorType">,
  read Group: EnumItem<"CreatorType">,
}
type CreatorTypeFilter = {
  read User: EnumItem<"CreatorTypeFilter">,
  read Group: EnumItem<"CreatorTypeFilter">,
  read All: EnumItem<"CreatorTypeFilter">,
}
type CurrencyType = {
  read Default: EnumItem<"CurrencyType">,
  read Robux: EnumItem<"CurrencyType">,
  read Tix: EnumItem<"CurrencyType">,
}
type CustomCameraMode = {
  read Default: EnumItem<"CustomCameraMode">,
  read Classic: EnumItem<"CustomCameraMode">,
  read Follow: EnumItem<"CustomCameraMode">,
}
type DataModelExtractorFileType = {
  read PlaceFile: EnumItem<"DataModelExtractorFileType">,
  read FirstSlice: EnumItem<"DataModelExtractorFileType">,
  read NonFirstSlice: EnumItem<"DataModelExtractorFileType">,
}
type DataStoreRequestType = {
  read GetAsync: EnumItem<"DataStoreRequestType">,
  read SetIncrementAsync: EnumItem<"DataStoreRequestType">,
  read UpdateAsync: EnumItem<"DataStoreRequestType">,
  read GetSortedAsync: EnumItem<"DataStoreRequestType">,
  read SetIncrementSortedAsync: EnumItem<"DataStoreRequestType">,
  read OnUpdate: EnumItem<"DataStoreRequestType">,
  read ListAsync: EnumItem<"DataStoreRequestType">,
  read GetVersionAsync: EnumItem<"DataStoreRequestType">,
  read RemoveVersionAsync: EnumItem<"DataStoreRequestType">,
  read StandardRead: EnumItem<"DataStoreRequestType">,
  read StandardWrite: EnumItem<"DataStoreRequestType">,
  read StandardList: EnumItem<"DataStoreRequestType">,
  read StandardRemove: EnumItem<"DataStoreRequestType">,
  read OrderedRead: EnumItem<"DataStoreRequestType">,
  read OrderedWrite: EnumItem<"DataStoreRequestType">,
  read OrderedList: EnumItem<"DataStoreRequestType">,
  read OrderedRemove: EnumItem<"DataStoreRequestType">,
}
type DebugBreakModeType = {
  read Never: EnumItem<"DebugBreakModeType">,
  read Always: EnumItem<"DebugBreakModeType">,
  read Unhandled: EnumItem<"DebugBreakModeType">,
}
type DebuggerEndReason = {
  read ClientRequest: EnumItem<"DebuggerEndReason">,
  read Timeout: EnumItem<"DebuggerEndReason">,
  read InvalidHost: EnumItem<"DebuggerEndReason">,
  read Disconnected: EnumItem<"DebuggerEndReason">,
  read ServerShutdown: EnumItem<"DebuggerEndReason">,
  read ServerProtocolMismatch: EnumItem<"DebuggerEndReason">,
  read ConfigurationFailed: EnumItem<"DebuggerEndReason">,
  read RpcError: EnumItem<"DebuggerEndReason">,
}
type DebuggerExceptionBreakMode = {
  read Never: EnumItem<"DebuggerExceptionBreakMode">,
  read Always: EnumItem<"DebuggerExceptionBreakMode">,
  read Unhandled: EnumItem<"DebuggerExceptionBreakMode">,
}
type DebuggerFrameType = {
  read C: EnumItem<"DebuggerFrameType">,
  read Lua: EnumItem<"DebuggerFrameType">,
}
type DebuggerPauseReason = {
  read Unknown: EnumItem<"DebuggerPauseReason">,
  read Requested: EnumItem<"DebuggerPauseReason">,
  read Breakpoint: EnumItem<"DebuggerPauseReason">,
  read Exception: EnumItem<"DebuggerPauseReason">,
  read SingleStep: EnumItem<"DebuggerPauseReason">,
  read Entrypoint: EnumItem<"DebuggerPauseReason">,
}
type DebuggerResumeType = {
  read StepInto: EnumItem<"DebuggerResumeType">,
  read StepOut: EnumItem<"DebuggerResumeType">,
  read StepOver: EnumItem<"DebuggerResumeType">,
  read Resume: EnumItem<"DebuggerResumeType">,
}
type DebuggerStatus = {
  read Success: EnumItem<"DebuggerStatus">,
  read Timeout: EnumItem<"DebuggerStatus">,
  read ConnectionLost: EnumItem<"DebuggerStatus">,
  read InvalidResponse: EnumItem<"DebuggerStatus">,
  read InternalError: EnumItem<"DebuggerStatus">,
  read InvalidState: EnumItem<"DebuggerStatus">,
  read RpcError: EnumItem<"DebuggerStatus">,
  read InvalidArgument: EnumItem<"DebuggerStatus">,
  read ConnectionClosed: EnumItem<"DebuggerStatus">,
}
type DefaultScriptSyncFileType = {
  read Lua: EnumItem<"DefaultScriptSyncFileType">,
  read Luau: EnumItem<"DefaultScriptSyncFileType">,
}
type DevCameraOcclusionMode = {
  read Zoom: EnumItem<"DevCameraOcclusionMode">,
  read Invisicam: EnumItem<"DevCameraOcclusionMode">,
}
type DevComputerCameraMovementMode = {
  read UserChoice: EnumItem<"DevComputerCameraMovementMode">,
  read Classic: EnumItem<"DevComputerCameraMovementMode">,
  read Follow: EnumItem<"DevComputerCameraMovementMode">,
  read Orbital: EnumItem<"DevComputerCameraMovementMode">,
  read CameraToggle: EnumItem<"DevComputerCameraMovementMode">,
}
type DevComputerMovementMode = {
  read UserChoice: EnumItem<"DevComputerMovementMode">,
  read KeyboardMouse: EnumItem<"DevComputerMovementMode">,
  read ClickToMove: EnumItem<"DevComputerMovementMode">,
  read Scriptable: EnumItem<"DevComputerMovementMode">,
}
type DevTouchCameraMovementMode = {
  read UserChoice: EnumItem<"DevTouchCameraMovementMode">,
  read Classic: EnumItem<"DevTouchCameraMovementMode">,
  read Follow: EnumItem<"DevTouchCameraMovementMode">,
  read Orbital: EnumItem<"DevTouchCameraMovementMode">,
}
type DevTouchMovementMode = {
  read UserChoice: EnumItem<"DevTouchMovementMode">,
  read Thumbstick: EnumItem<"DevTouchMovementMode">,
  read DPad: EnumItem<"DevTouchMovementMode">,
  read Thumbpad: EnumItem<"DevTouchMovementMode">,
  read ClickToMove: EnumItem<"DevTouchMovementMode">,
  read Scriptable: EnumItem<"DevTouchMovementMode">,
  read DynamicThumbstick: EnumItem<"DevTouchMovementMode">,
}
type DeveloperMemoryTag = {
  read Internal: EnumItem<"DeveloperMemoryTag">,
  read HttpCache: EnumItem<"DeveloperMemoryTag">,
  read Instances: EnumItem<"DeveloperMemoryTag">,
  read Signals: EnumItem<"DeveloperMemoryTag">,
  read LuaHeap: EnumItem<"DeveloperMemoryTag">,
  read Script: EnumItem<"DeveloperMemoryTag">,
  read PhysicsCollision: EnumItem<"DeveloperMemoryTag">,
  read BaseParts: EnumItem<"DeveloperMemoryTag">,
  read GraphicsSolidModels: EnumItem<"DeveloperMemoryTag">,
  read GraphicsMeshParts: EnumItem<"DeveloperMemoryTag">,
  read GraphicsParticles: EnumItem<"DeveloperMemoryTag">,
  read GraphicsParts: EnumItem<"DeveloperMemoryTag">,
  read GraphicsSpatialHash: EnumItem<"DeveloperMemoryTag">,
  read GraphicsTerrain: EnumItem<"DeveloperMemoryTag">,
  read GraphicsTexture: EnumItem<"DeveloperMemoryTag">,
  read GraphicsTextureCharacter: EnumItem<"DeveloperMemoryTag">,
  read Sounds: EnumItem<"DeveloperMemoryTag">,
  read StreamingSounds: EnumItem<"DeveloperMemoryTag">,
  read TerrainVoxels: EnumItem<"DeveloperMemoryTag">,
  read Gui: EnumItem<"DeveloperMemoryTag">,
  read Animation: EnumItem<"DeveloperMemoryTag">,
  read Navigation: EnumItem<"DeveloperMemoryTag">,
  read GeometryCSG: EnumItem<"DeveloperMemoryTag">,
  read GraphicsSlimModels: EnumItem<"DeveloperMemoryTag">,
}
type DeviceFeatureType = {
  read DeviceCapture: EnumItem<"DeviceFeatureType">,
  read InExperienceFAE: EnumItem<"DeviceFeatureType">,
}
type DeviceForm = {
  read Console: EnumItem<"DeviceForm">,
  read Phone: EnumItem<"DeviceForm">,
  read Tablet: EnumItem<"DeviceForm">,
  read Desktop: EnumItem<"DeviceForm">,
  read VR: EnumItem<"DeviceForm">,
}
type DeviceLevel = {
  read Low: EnumItem<"DeviceLevel">,
  read Medium: EnumItem<"DeviceLevel">,
  read High: EnumItem<"DeviceLevel">,
}
type DeviceSimulatorScalingMode = {
  read ScaleToPhysicalSize: EnumItem<"DeviceSimulatorScalingMode">,
  read ActualResolution: EnumItem<"DeviceSimulatorScalingMode">,
  read FitToWindow: EnumItem<"DeviceSimulatorScalingMode">,
}
type DeviceType = {
  read Unknown: EnumItem<"DeviceType">,
  read Desktop: EnumItem<"DeviceType">,
  read Tablet: EnumItem<"DeviceType">,
  read Phone: EnumItem<"DeviceType">,
  read TV: EnumItem<"DeviceType">,
}
type DialogBehaviorType = {
  read SinglePlayer: EnumItem<"DialogBehaviorType">,
  read MultiplePlayers: EnumItem<"DialogBehaviorType">,
}
type DialogPurpose = {
  read Quest: EnumItem<"DialogPurpose">,
  read Help: EnumItem<"DialogPurpose">,
  read Shop: EnumItem<"DialogPurpose">,
}
type DialogTone = {
  read Neutral: EnumItem<"DialogTone">,
  read Friendly: EnumItem<"DialogTone">,
  read Enemy: EnumItem<"DialogTone">,
}
type DigitsRigDescriptionSide = {
  read None: EnumItem<"DigitsRigDescriptionSide">,
  read Left: EnumItem<"DigitsRigDescriptionSide">,
  read Right: EnumItem<"DigitsRigDescriptionSide">,
}
type DiscountType = {
  read Uncategorized: EnumItem<"DiscountType">,
}
type DisplayScalingMode = {
  read Default: EnumItem<"DisplayScalingMode">,
  read Legacy: EnumItem<"DisplayScalingMode">,
  read Responsive: EnumItem<"DisplayScalingMode">,
}
type DisplaySize = {
  read Small: EnumItem<"DisplaySize">,
  read Medium: EnumItem<"DisplaySize">,
  read Large: EnumItem<"DisplaySize">,
}
type DomainType = {
  read EXPERIENCE: EnumItem<"DomainType">,
  read OAUTH: EnumItem<"DomainType">,
}
type DominantAxis = {
  read Width: EnumItem<"DominantAxis">,
  read Height: EnumItem<"DominantAxis">,
}
type DraftStatusCode = {
  read OK: EnumItem<"DraftStatusCode">,
  read DraftOutdated: EnumItem<"DraftStatusCode">,
  read ScriptRemoved: EnumItem<"DraftStatusCode">,
  read DraftCommitted: EnumItem<"DraftStatusCode">,
}
type DragDetectorDragStyle = {
  read TranslateLine: EnumItem<"DragDetectorDragStyle">,
  read TranslatePlane: EnumItem<"DragDetectorDragStyle">,
  read TranslatePlaneOrLine: EnumItem<"DragDetectorDragStyle">,
  read TranslateLineOrPlane: EnumItem<"DragDetectorDragStyle">,
  read TranslateViewPlane: EnumItem<"DragDetectorDragStyle">,
  read RotateAxis: EnumItem<"DragDetectorDragStyle">,
  read RotateTrackball: EnumItem<"DragDetectorDragStyle">,
  read Scriptable: EnumItem<"DragDetectorDragStyle">,
  read BestForDevice: EnumItem<"DragDetectorDragStyle">,
}
type DragDetectorPermissionPolicy = {
  read Nobody: EnumItem<"DragDetectorPermissionPolicy">,
  read Everybody: EnumItem<"DragDetectorPermissionPolicy">,
  read Scriptable: EnumItem<"DragDetectorPermissionPolicy">,
}
type DragDetectorResponseStyle = {
  read Geometric: EnumItem<"DragDetectorResponseStyle">,
  read Physical: EnumItem<"DragDetectorResponseStyle">,
  read Custom: EnumItem<"DragDetectorResponseStyle">,
}
type DraggerCoordinateSpace = {
  read Object: EnumItem<"DraggerCoordinateSpace">,
  read World: EnumItem<"DraggerCoordinateSpace">,
}
type DraggerMovementMode = {
  read Geometric: EnumItem<"DraggerMovementMode">,
  read Physical: EnumItem<"DraggerMovementMode">,
}
type DraggingScrollBar = {
  read None: EnumItem<"DraggingScrollBar">,
  read Horizontal: EnumItem<"DraggingScrollBar">,
  read Vertical: EnumItem<"DraggingScrollBar">,
}
type EasingDirection = {
  read In: EnumItem<"EasingDirection">,
  read Out: EnumItem<"EasingDirection">,
  read InOut: EnumItem<"EasingDirection">,
}
type EasingStyle = {
  read Linear: EnumItem<"EasingStyle">,
  read Sine: EnumItem<"EasingStyle">,
  read Back: EnumItem<"EasingStyle">,
  read Quad: EnumItem<"EasingStyle">,
  read Quart: EnumItem<"EasingStyle">,
  read Quint: EnumItem<"EasingStyle">,
  read Bounce: EnumItem<"EasingStyle">,
  read Elastic: EnumItem<"EasingStyle">,
  read Exponential: EnumItem<"EasingStyle">,
  read Circular: EnumItem<"EasingStyle">,
  read Cubic: EnumItem<"EasingStyle">,
}
type EditableStatus = {
  read Unknown: EnumItem<"EditableStatus">,
  read Allowed: EnumItem<"EditableStatus">,
  read Disallowed: EnumItem<"EditableStatus">,
}
type ElasticBehavior = {
  read WhenScrollable: EnumItem<"ElasticBehavior">,
  read Always: EnumItem<"ElasticBehavior">,
  read Never: EnumItem<"ElasticBehavior">,
}
type EmitterPositionType = {
  read Parent: EnumItem<"EmitterPositionType">,
  read Instance: EnumItem<"EmitterPositionType">,
}
type EngagementLevel = {
  read Unknown: EnumItem<"EngagementLevel">,
  read Inactive: EnumItem<"EngagementLevel">,
  read Low: EnumItem<"EngagementLevel">,
  read Medium: EnumItem<"EngagementLevel">,
  read High: EnumItem<"EngagementLevel">,
}
type EngineFolder = {
  read Screenshots: EnumItem<"EngineFolder">,
  read Videos: EnumItem<"EngineFolder">,
  read Logs: EnumItem<"EngineFolder">,
}
type EnviromentalPhysicsThrottle = {
  read DefaultAuto: EnumItem<"EnviromentalPhysicsThrottle">,
  read Disabled: EnumItem<"EnviromentalPhysicsThrottle">,
  read Always: EnumItem<"EnviromentalPhysicsThrottle">,
  read Skip2: EnumItem<"EnviromentalPhysicsThrottle">,
  read Skip4: EnumItem<"EnviromentalPhysicsThrottle">,
  read Skip8: EnumItem<"EnviromentalPhysicsThrottle">,
  read Skip16: EnumItem<"EnviromentalPhysicsThrottle">,
}
type ExperienceActivationStatus = {
  read Unknown: EnumItem<"ExperienceActivationStatus">,
  read New: EnumItem<"ExperienceActivationStatus">,
  read Active: EnumItem<"ExperienceActivationStatus">,
  read Lapsed: EnumItem<"ExperienceActivationStatus">,
  read Reactivated: EnumItem<"ExperienceActivationStatus">,
}
type ExperienceAuthScope = {
  read DefaultScope: EnumItem<"ExperienceAuthScope">,
  read CreatorAssetsCreate: EnumItem<"ExperienceAuthScope">,
}
type ExperienceEventStatus = {
  read Active: EnumItem<"ExperienceEventStatus">,
  read Cancelled: EnumItem<"ExperienceEventStatus">,
  read Moderated: EnumItem<"ExperienceEventStatus">,
  read Unpublished: EnumItem<"ExperienceEventStatus">,
  read Unknown: EnumItem<"ExperienceEventStatus">,
}
type ExperienceStateCaptureSelectionMode = {
  read Default: EnumItem<"ExperienceStateCaptureSelectionMode">,
  read SafetyHighlightMode: EnumItem<"ExperienceStateCaptureSelectionMode">,
}
type ExperienceStateRecordingLoadMode = {
  read NewReplay: EnumItem<"ExperienceStateRecordingLoadMode">,
  read ContiguousSlice: EnumItem<"ExperienceStateRecordingLoadMode">,
  read NoncontiguousSlice: EnumItem<"ExperienceStateRecordingLoadMode">,
}
type ExperienceStateRecordingLoadSourceType = {
  read S3Url: EnumItem<"ExperienceStateRecordingLoadSourceType">,
  read File: EnumItem<"ExperienceStateRecordingLoadSourceType">,
}
type ExperienceStateRecordingPlaybackMode = {
  read Undefined: EnumItem<"ExperienceStateRecordingPlaybackMode">,
  read Stopped: EnumItem<"ExperienceStateRecordingPlaybackMode">,
  read Playing: EnumItem<"ExperienceStateRecordingPlaybackMode">,
  read Rewinding: EnumItem<"ExperienceStateRecordingPlaybackMode">,
}
type ExplosionType = {
  read NoCraters: EnumItem<"ExplosionType">,
  read Craters: EnumItem<"ExplosionType">,
}
type ExternalEditorMode = {
  read SystemDefault: EnumItem<"ExternalEditorMode">,
  read UserSelectedEditor: EnumItem<"ExternalEditorMode">,
}
type FACSDataLod = {
  read LOD0: EnumItem<"FACSDataLod">,
  read LOD1: EnumItem<"FACSDataLod">,
  read LODCount: EnumItem<"FACSDataLod">,
}
type FacialAgeEstimationResultType = {
  read Complete: EnumItem<"FacialAgeEstimationResultType">,
  read Cancel: EnumItem<"FacialAgeEstimationResultType">,
  read Error: EnumItem<"FacialAgeEstimationResultType">,
}
type FacialAnimationStreamingState = {
  read None: EnumItem<"FacialAnimationStreamingState">,
  read Audio: EnumItem<"FacialAnimationStreamingState">,
  read Video: EnumItem<"FacialAnimationStreamingState">,
  read Place: EnumItem<"FacialAnimationStreamingState">,
  read Server: EnumItem<"FacialAnimationStreamingState">,
}
type FacsActionUnit = {
  read ChinRaiserUpperLip: EnumItem<"FacsActionUnit">,
  read ChinRaiser: EnumItem<"FacsActionUnit">,
  read FlatPucker: EnumItem<"FacsActionUnit">,
  read Funneler: EnumItem<"FacsActionUnit">,
  read LowerLipSuck: EnumItem<"FacsActionUnit">,
  read LipPresser: EnumItem<"FacsActionUnit">,
  read LipsTogether: EnumItem<"FacsActionUnit">,
  read MouthLeft: EnumItem<"FacsActionUnit">,
  read MouthRight: EnumItem<"FacsActionUnit">,
  read Pucker: EnumItem<"FacsActionUnit">,
  read UpperLipSuck: EnumItem<"FacsActionUnit">,
  read LeftCheekPuff: EnumItem<"FacsActionUnit">,
  read LeftDimpler: EnumItem<"FacsActionUnit">,
  read LeftLipCornerDown: EnumItem<"FacsActionUnit">,
  read LeftLowerLipDepressor: EnumItem<"FacsActionUnit">,
  read LeftLipCornerPuller: EnumItem<"FacsActionUnit">,
  read LeftLipStretcher: EnumItem<"FacsActionUnit">,
  read LeftUpperLipRaiser: EnumItem<"FacsActionUnit">,
  read RightCheekPuff: EnumItem<"FacsActionUnit">,
  read RightDimpler: EnumItem<"FacsActionUnit">,
  read RightLipCornerDown: EnumItem<"FacsActionUnit">,
  read RightLowerLipDepressor: EnumItem<"FacsActionUnit">,
  read RightLipCornerPuller: EnumItem<"FacsActionUnit">,
  read RightLipStretcher: EnumItem<"FacsActionUnit">,
  read RightUpperLipRaiser: EnumItem<"FacsActionUnit">,
  read JawDrop: EnumItem<"FacsActionUnit">,
  read JawLeft: EnumItem<"FacsActionUnit">,
  read JawRight: EnumItem<"FacsActionUnit">,
  read Corrugator: EnumItem<"FacsActionUnit">,
  read LeftBrowLowerer: EnumItem<"FacsActionUnit">,
  read LeftOuterBrowRaiser: EnumItem<"FacsActionUnit">,
  read LeftNoseWrinkler: EnumItem<"FacsActionUnit">,
  read LeftInnerBrowRaiser: EnumItem<"FacsActionUnit">,
  read RightBrowLowerer: EnumItem<"FacsActionUnit">,
  read RightOuterBrowRaiser: EnumItem<"FacsActionUnit">,
  read RightInnerBrowRaiser: EnumItem<"FacsActionUnit">,
  read RightNoseWrinkler: EnumItem<"FacsActionUnit">,
  read EyesLookDown: EnumItem<"FacsActionUnit">,
  read EyesLookLeft: EnumItem<"FacsActionUnit">,
  read EyesLookUp: EnumItem<"FacsActionUnit">,
  read EyesLookRight: EnumItem<"FacsActionUnit">,
  read LeftCheekRaiser: EnumItem<"FacsActionUnit">,
  read LeftEyeUpperLidRaiser: EnumItem<"FacsActionUnit">,
  read LeftEyeClosed: EnumItem<"FacsActionUnit">,
  read RightCheekRaiser: EnumItem<"FacsActionUnit">,
  read RightEyeUpperLidRaiser: EnumItem<"FacsActionUnit">,
  read RightEyeClosed: EnumItem<"FacsActionUnit">,
  read TongueDown: EnumItem<"FacsActionUnit">,
  read TongueOut: EnumItem<"FacsActionUnit">,
  read TongueUp: EnumItem<"FacsActionUnit">,
}
type FeatureRestrictionAbuseVector = {
  read ExperienceChat: EnumItem<"FeatureRestrictionAbuseVector">,
  read Communication: EnumItem<"FeatureRestrictionAbuseVector">,
}
type FeedbackType = {
  read Feedback: EnumItem<"FeedbackType">,
  read PlayerSupport: EnumItem<"FeedbackType">,
}
type FieldOfViewMode = {
  read Vertical: EnumItem<"FieldOfViewMode">,
  read Diagonal: EnumItem<"FieldOfViewMode">,
  read MaxAxis: EnumItem<"FieldOfViewMode">,
}
type FillDirection = {
  read Horizontal: EnumItem<"FillDirection">,
  read Vertical: EnumItem<"FillDirection">,
}
type FilterErrorType = {
  read BackslashNotEscapingAnything: EnumItem<"FilterErrorType">,
  read BadBespokeFilter: EnumItem<"FilterErrorType">,
  read BadName: EnumItem<"FilterErrorType">,
  read IncompleteOr: EnumItem<"FilterErrorType">,
  read IncompleteParenthesis: EnumItem<"FilterErrorType">,
  read InvalidDoubleStar: EnumItem<"FilterErrorType">,
  read InvalidTilde: EnumItem<"FilterErrorType">,
  read PropertyBadOperator: EnumItem<"FilterErrorType">,
  read PropertyDoesNotExist: EnumItem<"FilterErrorType">,
  read PropertyInvalidField: EnumItem<"FilterErrorType">,
  read PropertyInvalidValue: EnumItem<"FilterErrorType">,
  read PropertyUnsupportedFields: EnumItem<"FilterErrorType">,
  read PropertyUnsupportedProperty: EnumItem<"FilterErrorType">,
  read UnexpectedNameIndex: EnumItem<"FilterErrorType">,
  read UnexpectedToken: EnumItem<"FilterErrorType">,
  read UnfinishedBinaryOperator: EnumItem<"FilterErrorType">,
  read UnfinishedQuote: EnumItem<"FilterErrorType">,
  read UnknownBespokeFilter: EnumItem<"FilterErrorType">,
  read WildcardInProperty: EnumItem<"FilterErrorType">,
}
type FilterResult = {
  read Accepted: EnumItem<"FilterResult">,
  read Rejected: EnumItem<"FilterResult">,
}
type FilterType = {
  read Exclude: EnumItem<"FilterType">,
  read Include: EnumItem<"FilterType">,
}
type FinishRecordingOperation = {
  read Cancel: EnumItem<"FinishRecordingOperation">,
  read Commit: EnumItem<"FinishRecordingOperation">,
  read Append: EnumItem<"FinishRecordingOperation">,
}
type FluidFidelity = {
  read Automatic: EnumItem<"FluidFidelity">,
  read UseCollisionGeometry: EnumItem<"FluidFidelity">,
  read UsePreciseGeometry: EnumItem<"FluidFidelity">,
}
type FluidForces = {
  read Default: EnumItem<"FluidForces">,
  read Experimental: EnumItem<"FluidForces">,
}
type Font = {
  read Legacy: EnumItem<"Font">,
  read Arial: EnumItem<"Font">,
  read ArialBold: EnumItem<"Font">,
  read SourceSans: EnumItem<"Font">,
  read SourceSansBold: EnumItem<"Font">,
  read SourceSansLight: EnumItem<"Font">,
  read SourceSansItalic: EnumItem<"Font">,
  read Bodoni: EnumItem<"Font">,
  read Garamond: EnumItem<"Font">,
  read Cartoon: EnumItem<"Font">,
  read Code: EnumItem<"Font">,
  read Highway: EnumItem<"Font">,
  read SciFi: EnumItem<"Font">,
  read Arcade: EnumItem<"Font">,
  read Fantasy: EnumItem<"Font">,
  read Antique: EnumItem<"Font">,
  read SourceSansSemibold: EnumItem<"Font">,
  read Gotham: EnumItem<"Font">,
  read GothamMedium: EnumItem<"Font">,
  read GothamBold: EnumItem<"Font">,
  read GothamBlack: EnumItem<"Font">,
  read AmaticSC: EnumItem<"Font">,
  read Bangers: EnumItem<"Font">,
  read Creepster: EnumItem<"Font">,
  read DenkOne: EnumItem<"Font">,
  read Fondamento: EnumItem<"Font">,
  read FredokaOne: EnumItem<"Font">,
  read GrenzeGotisch: EnumItem<"Font">,
  read IndieFlower: EnumItem<"Font">,
  read JosefinSans: EnumItem<"Font">,
  read Jura: EnumItem<"Font">,
  read Kalam: EnumItem<"Font">,
  read LuckiestGuy: EnumItem<"Font">,
  read Merriweather: EnumItem<"Font">,
  read Michroma: EnumItem<"Font">,
  read Nunito: EnumItem<"Font">,
  read Oswald: EnumItem<"Font">,
  read PatrickHand: EnumItem<"Font">,
  read PermanentMarker: EnumItem<"Font">,
  read Roboto: EnumItem<"Font">,
  read RobotoCondensed: EnumItem<"Font">,
  read RobotoMono: EnumItem<"Font">,
  read Sarpanch: EnumItem<"Font">,
  read SpecialElite: EnumItem<"Font">,
  read TitilliumWeb: EnumItem<"Font">,
  read Ubuntu: EnumItem<"Font">,
  read BuilderSans: EnumItem<"Font">,
  read BuilderSansMedium: EnumItem<"Font">,
  read BuilderSansBold: EnumItem<"Font">,
  read BuilderSansExtraBold: EnumItem<"Font">,
  read Arimo: EnumItem<"Font">,
  read ArimoBold: EnumItem<"Font">,
  read Unknown: EnumItem<"Font">,
}
type FontSize = {
  read Size8: EnumItem<"FontSize">,
  read Size9: EnumItem<"FontSize">,
  read Size10: EnumItem<"FontSize">,
  read Size11: EnumItem<"FontSize">,
  read Size12: EnumItem<"FontSize">,
  read Size14: EnumItem<"FontSize">,
  read Size18: EnumItem<"FontSize">,
  read Size24: EnumItem<"FontSize">,
  read Size36: EnumItem<"FontSize">,
  read Size48: EnumItem<"FontSize">,
  read Size28: EnumItem<"FontSize">,
  read Size32: EnumItem<"FontSize">,
  read Size42: EnumItem<"FontSize">,
  read Size60: EnumItem<"FontSize">,
  read Size96: EnumItem<"FontSize">,
}
type FontStyle = {
  read Normal: EnumItem<"FontStyle">,
  read Italic: EnumItem<"FontStyle">,
}
type FontWeight = {
  read Thin: EnumItem<"FontWeight">,
  read ExtraLight: EnumItem<"FontWeight">,
  read Light: EnumItem<"FontWeight">,
  read Regular: EnumItem<"FontWeight">,
  read Medium: EnumItem<"FontWeight">,
  read SemiBold: EnumItem<"FontWeight">,
  read Bold: EnumItem<"FontWeight">,
  read ExtraBold: EnumItem<"FontWeight">,
  read Heavy: EnumItem<"FontWeight">,
}
type ForceLimitMode = {
  read Magnitude: EnumItem<"ForceLimitMode">,
  read PerAxis: EnumItem<"ForceLimitMode">,
}
type FormFactor = {
  read Symmetric: EnumItem<"FormFactor">,
  read Brick: EnumItem<"FormFactor">,
  read Plate: EnumItem<"FormFactor">,
  read Custom: EnumItem<"FormFactor">,
}
type FrameStyle = {
  read Custom: EnumItem<"FrameStyle">,
  read ChatBlue: EnumItem<"FrameStyle">,
  read RobloxSquare: EnumItem<"FrameStyle">,
  read RobloxRound: EnumItem<"FrameStyle">,
  read ChatGreen: EnumItem<"FrameStyle">,
  read ChatRed: EnumItem<"FrameStyle">,
  read DropShadow: EnumItem<"FrameStyle">,
}
type FramerateManagerMode = {
  read Automatic: EnumItem<"FramerateManagerMode">,
  read On: EnumItem<"FramerateManagerMode">,
  read Off: EnumItem<"FramerateManagerMode">,
}
type FriendRequestEvent = {
  read Issue: EnumItem<"FriendRequestEvent">,
  read Revoke: EnumItem<"FriendRequestEvent">,
  read Accept: EnumItem<"FriendRequestEvent">,
  read Deny: EnumItem<"FriendRequestEvent">,
}
type FriendStatus = {
  read Unknown: EnumItem<"FriendStatus">,
  read NotFriend: EnumItem<"FriendStatus">,
  read Friend: EnumItem<"FriendStatus">,
  read FriendRequestSent: EnumItem<"FriendStatus">,
  read FriendRequestReceived: EnumItem<"FriendStatus">,
}
type FunctionalTestResult = {
  read Passed: EnumItem<"FunctionalTestResult">,
  read Warning: EnumItem<"FunctionalTestResult">,
  read Error: EnumItem<"FunctionalTestResult">,
}
type GameAvatarType = {
  read R6: EnumItem<"GameAvatarType">,
  read R15: EnumItem<"GameAvatarType">,
  read PlayerChoice: EnumItem<"GameAvatarType">,
}
type GamepadType = {
  read Unknown: EnumItem<"GamepadType">,
  read PS4: EnumItem<"GamepadType">,
  read PS5: EnumItem<"GamepadType">,
  read XboxOne: EnumItem<"GamepadType">,
}
type GearGenreSetting = {
  read AllGenres: EnumItem<"GearGenreSetting">,
  read MatchingGenreOnly: EnumItem<"GearGenreSetting">,
}
type GearType = {
  read MeleeWeapons: EnumItem<"GearType">,
  read RangedWeapons: EnumItem<"GearType">,
  read Explosives: EnumItem<"GearType">,
  read PowerUps: EnumItem<"GearType">,
  read NavigationEnhancers: EnumItem<"GearType">,
  read MusicalInstruments: EnumItem<"GearType">,
  read SocialItems: EnumItem<"GearType">,
  read BuildingTools: EnumItem<"GearType">,
  read Transport: EnumItem<"GearType">,
}
type Genre = {
  read All: EnumItem<"Genre">,
  read TownAndCity: EnumItem<"Genre">,
  read Fantasy: EnumItem<"Genre">,
  read SciFi: EnumItem<"Genre">,
  read Ninja: EnumItem<"Genre">,
  read Scary: EnumItem<"Genre">,
  read Pirate: EnumItem<"Genre">,
  read Adventure: EnumItem<"Genre">,
  read Sports: EnumItem<"Genre">,
  read Funny: EnumItem<"Genre">,
  read WildWest: EnumItem<"Genre">,
  read War: EnumItem<"Genre">,
  read SkatePark: EnumItem<"Genre">,
  read Tutorial: EnumItem<"Genre">,
}
type GradientTileMode = {
  read Clamp: EnumItem<"GradientTileMode">,
  read Repeat: EnumItem<"GradientTileMode">,
  read Mirror: EnumItem<"GradientTileMode">,
}
type GradientType = {
  read Linear: EnumItem<"GradientType">,
  read Radial: EnumItem<"GradientType">,
  read Conical: EnumItem<"GradientType">,
}
type GraphicsMode = {
  read Automatic: EnumItem<"GraphicsMode">,
  read Direct3D11: EnumItem<"GraphicsMode">,
  read OpenGL: EnumItem<"GraphicsMode">,
  read Metal: EnumItem<"GraphicsMode">,
  read Vulkan: EnumItem<"GraphicsMode">,
  read NoGraphics: EnumItem<"GraphicsMode">,
}
type GraphicsOptimizationMode = {
  read Performance: EnumItem<"GraphicsOptimizationMode">,
  read Balanced: EnumItem<"GraphicsOptimizationMode">,
  read Quality: EnumItem<"GraphicsOptimizationMode">,
}
type GroupMembershipStatus = {
  read None: EnumItem<"GroupMembershipStatus">,
  read Joined: EnumItem<"GroupMembershipStatus">,
  read JoinRequestPending: EnumItem<"GroupMembershipStatus">,
  read AlreadyMember: EnumItem<"GroupMembershipStatus">,
}
type GuiState = {
  read Idle: EnumItem<"GuiState">,
  read Hover: EnumItem<"GuiState">,
  read Press: EnumItem<"GuiState">,
  read NonInteractable: EnumItem<"GuiState">,
}
type GuiType = {
  read Core: EnumItem<"GuiType">,
  read Custom: EnumItem<"GuiType">,
  read PlayerNameplates: EnumItem<"GuiType">,
  read CustomBillboards: EnumItem<"GuiType">,
  read CoreBillboards: EnumItem<"GuiType">,
}
type HandlesStyle = {
  read Resize: EnumItem<"HandlesStyle">,
  read Movement: EnumItem<"HandlesStyle">,
}
type HapticEffectType = {
  read Custom: EnumItem<"HapticEffectType">,
  read UIHover: EnumItem<"HapticEffectType">,
  read UIClick: EnumItem<"HapticEffectType">,
  read UINotification: EnumItem<"HapticEffectType">,
  read GameplayExplosion: EnumItem<"HapticEffectType">,
  read GameplayCollision: EnumItem<"HapticEffectType">,
}
type HashAlgorithm = {
  read Blake2b: EnumItem<"HashAlgorithm">,
  read Blake3: EnumItem<"HashAlgorithm">,
  read Md5: EnumItem<"HashAlgorithm">,
  read Sha1: EnumItem<"HashAlgorithm">,
  read Sha256: EnumItem<"HashAlgorithm">,
}
type HighlightDepthMode = {
  read AlwaysOnTop: EnumItem<"HighlightDepthMode">,
  read Occluded: EnumItem<"HighlightDepthMode">,
}
type HorizontalAlignment = {
  read Center: EnumItem<"HorizontalAlignment">,
  read Left: EnumItem<"HorizontalAlignment">,
  read Right: EnumItem<"HorizontalAlignment">,
}
type HoverAnimateSpeed = {
  read VerySlow: EnumItem<"HoverAnimateSpeed">,
  read Slow: EnumItem<"HoverAnimateSpeed">,
  read Medium: EnumItem<"HoverAnimateSpeed">,
  read Fast: EnumItem<"HoverAnimateSpeed">,
  read VeryFast: EnumItem<"HoverAnimateSpeed">,
}
type HttpCachePolicy = {
  read None: EnumItem<"HttpCachePolicy">,
  read Full: EnumItem<"HttpCachePolicy">,
  read DataOnly: EnumItem<"HttpCachePolicy">,
  read Default: EnumItem<"HttpCachePolicy">,
  read InternalRedirectRefresh: EnumItem<"HttpCachePolicy">,
}
type HttpCompression = {
  read None: EnumItem<"HttpCompression">,
  read Gzip: EnumItem<"HttpCompression">,
}
type HttpContentType = {
  read ApplicationJson: EnumItem<"HttpContentType">,
  read ApplicationXml: EnumItem<"HttpContentType">,
  read ApplicationUrlEncoded: EnumItem<"HttpContentType">,
  read TextPlain: EnumItem<"HttpContentType">,
  read TextXml: EnumItem<"HttpContentType">,
}
type HttpError = {
  read OK: EnumItem<"HttpError">,
  read InvalidUrl: EnumItem<"HttpError">,
  read DnsResolve: EnumItem<"HttpError">,
  read ConnectFail: EnumItem<"HttpError">,
  read OutOfMemory: EnumItem<"HttpError">,
  read TimedOut: EnumItem<"HttpError">,
  read TooManyRedirects: EnumItem<"HttpError">,
  read InvalidRedirect: EnumItem<"HttpError">,
  read NetFail: EnumItem<"HttpError">,
  read Aborted: EnumItem<"HttpError">,
  read SslConnectFail: EnumItem<"HttpError">,
  read SslVerificationFail: EnumItem<"HttpError">,
  read Unknown: EnumItem<"HttpError">,
  read ConnectionClosed: EnumItem<"HttpError">,
  read ServerProtocolError: EnumItem<"HttpError">,
  read CreatorEnvironmentsNotSupportedByService: EnumItem<"HttpError">,
  read InactivityTimeout: EnumItem<"HttpError">,
  read TooManyOutstandingRequests: EnumItem<"HttpError">,
  read InvalidRangeResponse: EnumItem<"HttpError">,
}
type HttpRequestType = {
  read Default: EnumItem<"HttpRequestType">,
  read MarketplaceService: EnumItem<"HttpRequestType">,
  read Players: EnumItem<"HttpRequestType">,
  read Chat: EnumItem<"HttpRequestType">,
  read Avatar: EnumItem<"HttpRequestType">,
  read Analytics: EnumItem<"HttpRequestType">,
  read Localization: EnumItem<"HttpRequestType">,
}
type HumanoidCollisionType = {
  read OuterBox: EnumItem<"HumanoidCollisionType">,
  read InnerBox: EnumItem<"HumanoidCollisionType">,
}
type HumanoidDisplayDistanceType = {
  read Viewer: EnumItem<"HumanoidDisplayDistanceType">,
  read Subject: EnumItem<"HumanoidDisplayDistanceType">,
  read None: EnumItem<"HumanoidDisplayDistanceType">,
}
type HumanoidHealthDisplayType = {
  read DisplayWhenDamaged: EnumItem<"HumanoidHealthDisplayType">,
  read AlwaysOn: EnumItem<"HumanoidHealthDisplayType">,
  read AlwaysOff: EnumItem<"HumanoidHealthDisplayType">,
}
type HumanoidRigType = {
  read R6: EnumItem<"HumanoidRigType">,
  read R15: EnumItem<"HumanoidRigType">,
}
type HumanoidStateType = {
  read FallingDown: EnumItem<"HumanoidStateType">,
  read Ragdoll: EnumItem<"HumanoidStateType">,
  read GettingUp: EnumItem<"HumanoidStateType">,
  read Jumping: EnumItem<"HumanoidStateType">,
  read Swimming: EnumItem<"HumanoidStateType">,
  read Freefall: EnumItem<"HumanoidStateType">,
  read Flying: EnumItem<"HumanoidStateType">,
  read Landed: EnumItem<"HumanoidStateType">,
  read Running: EnumItem<"HumanoidStateType">,
  read RunningNoPhysics: EnumItem<"HumanoidStateType">,
  read StrafingNoPhysics: EnumItem<"HumanoidStateType">,
  read Climbing: EnumItem<"HumanoidStateType">,
  read Seated: EnumItem<"HumanoidStateType">,
  read PlatformStanding: EnumItem<"HumanoidStateType">,
  read Dead: EnumItem<"HumanoidStateType">,
  read Physics: EnumItem<"HumanoidStateType">,
  read None: EnumItem<"HumanoidStateType">,
}
type IKCollisionsMode = {
  read NoCollisions: EnumItem<"IKCollisionsMode">,
  read OtherMechanismsAnchored: EnumItem<"IKCollisionsMode">,
  read IncludeContactedMechanisms: EnumItem<"IKCollisionsMode">,
}
type IKControlConstraintSupport = {
  read Default: EnumItem<"IKControlConstraintSupport">,
  read Disabled: EnumItem<"IKControlConstraintSupport">,
  read Enabled: EnumItem<"IKControlConstraintSupport">,
}
type IKControlType = {
  read Transform: EnumItem<"IKControlType">,
  read Position: EnumItem<"IKControlType">,
  read Rotation: EnumItem<"IKControlType">,
  read LookAt: EnumItem<"IKControlType">,
}
type IXPLoadingStatus = {
  read None: EnumItem<"IXPLoadingStatus">,
  read Pending: EnumItem<"IXPLoadingStatus">,
  read Initialized: EnumItem<"IXPLoadingStatus">,
  read ErrorInvalidUser: EnumItem<"IXPLoadingStatus">,
  read ErrorConnection: EnumItem<"IXPLoadingStatus">,
  read ErrorJsonParse: EnumItem<"IXPLoadingStatus">,
  read ErrorTimedOut: EnumItem<"IXPLoadingStatus">,
}
type ImageAlphaType = {
  read Default: EnumItem<"ImageAlphaType">,
  read LockCanvasAlpha: EnumItem<"ImageAlphaType">,
  read LockCanvasColor: EnumItem<"ImageAlphaType">,
}
type ImageCombineType = {
  read BlendSourceOver: EnumItem<"ImageCombineType">,
  read Overwrite: EnumItem<"ImageCombineType">,
  read Add: EnumItem<"ImageCombineType">,
  read Multiply: EnumItem<"ImageCombineType">,
  read AlphaBlend: EnumItem<"ImageCombineType">,
  read NormalMapBlend: EnumItem<"ImageCombineType">,
}
type InOut = {
  read Edge: EnumItem<"InOut">,
  read Inset: EnumItem<"InOut">,
  read Center: EnumItem<"InOut">,
}
type InfoType = {
  read Asset: EnumItem<"InfoType">,
  read Product: EnumItem<"InfoType">,
  read GamePass: EnumItem<"InfoType">,
  read Subscription: EnumItem<"InfoType">,
  read Bundle: EnumItem<"InfoType">,
}
type InitialDockState = {
  read Top: EnumItem<"InitialDockState">,
  read Bottom: EnumItem<"InitialDockState">,
  read Left: EnumItem<"InitialDockState">,
  read Right: EnumItem<"InitialDockState">,
  read Float: EnumItem<"InitialDockState">,
}
type InputActionType = {
  read Bool: EnumItem<"InputActionType">,
  read Direction1D: EnumItem<"InputActionType">,
  read Direction2D: EnumItem<"InputActionType">,
  read Direction3D: EnumItem<"InputActionType">,
  read ViewportPosition: EnumItem<"InputActionType">,
}
type InputBindingType = {
  read Automatic: EnumItem<"InputBindingType">,
  read Scriptable: EnumItem<"InputBindingType">,
}
type InputSink = {
  read None: EnumItem<"InputSink">,
  read Activate: EnumItem<"InputSink">,
  read All: EnumItem<"InputSink">,
}
type InputType = {
  read NoInput: EnumItem<"InputType">,
  read Constant: EnumItem<"InputType">,
  read Sin: EnumItem<"InputType">,
}
type InstanceFileSyncStatus = {
  read NotSynced: EnumItem<"InstanceFileSyncStatus">,
  read Errored: EnumItem<"InstanceFileSyncStatus">,
  read SyncedAsRoot: EnumItem<"InstanceFileSyncStatus">,
  read SyncedAsDescendant: EnumItem<"InstanceFileSyncStatus">,
  read AncestorErrored: EnumItem<"InstanceFileSyncStatus">,
}
type IntermediateMeshGenerationResult = {
  read HighQualityMesh: EnumItem<"IntermediateMeshGenerationResult">,
}
type InternalVideoUsage = {
  read Default: EnumItem<"InternalVideoUsage">,
  read WatchPage: EnumItem<"InternalVideoUsage">,
  read HomeCarousel: EnumItem<"InternalVideoUsage">,
}
type InterpolationThrottlingMode = {
  read Default: EnumItem<"InterpolationThrottlingMode">,
  read Disabled: EnumItem<"InterpolationThrottlingMode">,
  read Enabled: EnumItem<"InterpolationThrottlingMode">,
}
type InviteState = {
  read Placed: EnumItem<"InviteState">,
  read Accepted: EnumItem<"InviteState">,
  read Declined: EnumItem<"InviteState">,
  read Missed: EnumItem<"InviteState">,
}
type ItemLineAlignment = {
  read Automatic: EnumItem<"ItemLineAlignment">,
  read Start: EnumItem<"ItemLineAlignment">,
  read Center: EnumItem<"ItemLineAlignment">,
  read End: EnumItem<"ItemLineAlignment">,
  read Stretch: EnumItem<"ItemLineAlignment">,
}
type JoinSource = {
  read CreatedItemAttribution: EnumItem<"JoinSource">,
}
type JointCreationMode = {
  read All: EnumItem<"JointCreationMode">,
  read Surface: EnumItem<"JointCreationMode">,
  read None: EnumItem<"JointCreationMode">,
}
type KeyCode = {
  read Unknown: EnumItem<"KeyCode">,
  read Backspace: EnumItem<"KeyCode">,
  read Tab: EnumItem<"KeyCode">,
  read Clear: EnumItem<"KeyCode">,
  read Return: EnumItem<"KeyCode">,
  read Pause: EnumItem<"KeyCode">,
  read Escape: EnumItem<"KeyCode">,
  read Space: EnumItem<"KeyCode">,
  read QuotedDouble: EnumItem<"KeyCode">,
  read Hash: EnumItem<"KeyCode">,
  read Dollar: EnumItem<"KeyCode">,
  read Percent: EnumItem<"KeyCode">,
  read Ampersand: EnumItem<"KeyCode">,
  read Quote: EnumItem<"KeyCode">,
  read LeftParenthesis: EnumItem<"KeyCode">,
  read RightParenthesis: EnumItem<"KeyCode">,
  read Asterisk: EnumItem<"KeyCode">,
  read Plus: EnumItem<"KeyCode">,
  read Comma: EnumItem<"KeyCode">,
  read Minus: EnumItem<"KeyCode">,
  read Period: EnumItem<"KeyCode">,
  read Slash: EnumItem<"KeyCode">,
  read Zero: EnumItem<"KeyCode">,
  read One: EnumItem<"KeyCode">,
  read Two: EnumItem<"KeyCode">,
  read Three: EnumItem<"KeyCode">,
  read Four: EnumItem<"KeyCode">,
  read Five: EnumItem<"KeyCode">,
  read Six: EnumItem<"KeyCode">,
  read Seven: EnumItem<"KeyCode">,
  read Eight: EnumItem<"KeyCode">,
  read Nine: EnumItem<"KeyCode">,
  read Colon: EnumItem<"KeyCode">,
  read Semicolon: EnumItem<"KeyCode">,
  read LessThan: EnumItem<"KeyCode">,
  read Equals: EnumItem<"KeyCode">,
  read GreaterThan: EnumItem<"KeyCode">,
  read Question: EnumItem<"KeyCode">,
  read At: EnumItem<"KeyCode">,
  read LeftBracket: EnumItem<"KeyCode">,
  read BackSlash: EnumItem<"KeyCode">,
  read RightBracket: EnumItem<"KeyCode">,
  read Caret: EnumItem<"KeyCode">,
  read Underscore: EnumItem<"KeyCode">,
  read Backquote: EnumItem<"KeyCode">,
  read A: EnumItem<"KeyCode">,
  read B: EnumItem<"KeyCode">,
  read C: EnumItem<"KeyCode">,
  read D: EnumItem<"KeyCode">,
  read E: EnumItem<"KeyCode">,
  read F: EnumItem<"KeyCode">,
  read G: EnumItem<"KeyCode">,
  read H: EnumItem<"KeyCode">,
  read I: EnumItem<"KeyCode">,
  read J: EnumItem<"KeyCode">,
  read K: EnumItem<"KeyCode">,
  read L: EnumItem<"KeyCode">,
  read M: EnumItem<"KeyCode">,
  read N: EnumItem<"KeyCode">,
  read O: EnumItem<"KeyCode">,
  read P: EnumItem<"KeyCode">,
  read Q: EnumItem<"KeyCode">,
  read R: EnumItem<"KeyCode">,
  read S: EnumItem<"KeyCode">,
  read T: EnumItem<"KeyCode">,
  read U: EnumItem<"KeyCode">,
  read V: EnumItem<"KeyCode">,
  read W: EnumItem<"KeyCode">,
  read X: EnumItem<"KeyCode">,
  read Y: EnumItem<"KeyCode">,
  read Z: EnumItem<"KeyCode">,
  read LeftCurly: EnumItem<"KeyCode">,
  read Pipe: EnumItem<"KeyCode">,
  read RightCurly: EnumItem<"KeyCode">,
  read Tilde: EnumItem<"KeyCode">,
  read Delete: EnumItem<"KeyCode">,
  read World0: EnumItem<"KeyCode">,
  read World1: EnumItem<"KeyCode">,
  read World2: EnumItem<"KeyCode">,
  read World3: EnumItem<"KeyCode">,
  read World4: EnumItem<"KeyCode">,
  read World5: EnumItem<"KeyCode">,
  read World6: EnumItem<"KeyCode">,
  read World7: EnumItem<"KeyCode">,
  read World8: EnumItem<"KeyCode">,
  read World9: EnumItem<"KeyCode">,
  read World10: EnumItem<"KeyCode">,
  read World11: EnumItem<"KeyCode">,
  read World12: EnumItem<"KeyCode">,
  read World13: EnumItem<"KeyCode">,
  read World14: EnumItem<"KeyCode">,
  read World15: EnumItem<"KeyCode">,
  read World16: EnumItem<"KeyCode">,
  read World17: EnumItem<"KeyCode">,
  read World18: EnumItem<"KeyCode">,
  read World19: EnumItem<"KeyCode">,
  read World20: EnumItem<"KeyCode">,
  read World21: EnumItem<"KeyCode">,
  read World22: EnumItem<"KeyCode">,
  read World23: EnumItem<"KeyCode">,
  read World24: EnumItem<"KeyCode">,
  read World25: EnumItem<"KeyCode">,
  read World26: EnumItem<"KeyCode">,
  read World27: EnumItem<"KeyCode">,
  read World28: EnumItem<"KeyCode">,
  read World29: EnumItem<"KeyCode">,
  read World30: EnumItem<"KeyCode">,
  read World31: EnumItem<"KeyCode">,
  read World32: EnumItem<"KeyCode">,
  read World33: EnumItem<"KeyCode">,
  read World34: EnumItem<"KeyCode">,
  read World35: EnumItem<"KeyCode">,
  read World36: EnumItem<"KeyCode">,
  read World37: EnumItem<"KeyCode">,
  read World38: EnumItem<"KeyCode">,
  read World39: EnumItem<"KeyCode">,
  read World40: EnumItem<"KeyCode">,
  read World41: EnumItem<"KeyCode">,
  read World42: EnumItem<"KeyCode">,
  read World43: EnumItem<"KeyCode">,
  read World44: EnumItem<"KeyCode">,
  read World45: EnumItem<"KeyCode">,
  read World46: EnumItem<"KeyCode">,
  read World47: EnumItem<"KeyCode">,
  read World48: EnumItem<"KeyCode">,
  read World49: EnumItem<"KeyCode">,
  read World50: EnumItem<"KeyCode">,
  read World51: EnumItem<"KeyCode">,
  read World52: EnumItem<"KeyCode">,
  read World53: EnumItem<"KeyCode">,
  read World54: EnumItem<"KeyCode">,
  read World55: EnumItem<"KeyCode">,
  read World56: EnumItem<"KeyCode">,
  read World57: EnumItem<"KeyCode">,
  read World58: EnumItem<"KeyCode">,
  read World59: EnumItem<"KeyCode">,
  read World60: EnumItem<"KeyCode">,
  read World61: EnumItem<"KeyCode">,
  read World62: EnumItem<"KeyCode">,
  read World63: EnumItem<"KeyCode">,
  read World64: EnumItem<"KeyCode">,
  read World65: EnumItem<"KeyCode">,
  read World66: EnumItem<"KeyCode">,
  read World67: EnumItem<"KeyCode">,
  read World68: EnumItem<"KeyCode">,
  read World69: EnumItem<"KeyCode">,
  read World70: EnumItem<"KeyCode">,
  read World71: EnumItem<"KeyCode">,
  read World72: EnumItem<"KeyCode">,
  read World73: EnumItem<"KeyCode">,
  read World74: EnumItem<"KeyCode">,
  read World75: EnumItem<"KeyCode">,
  read World76: EnumItem<"KeyCode">,
  read World77: EnumItem<"KeyCode">,
  read World78: EnumItem<"KeyCode">,
  read World79: EnumItem<"KeyCode">,
  read World80: EnumItem<"KeyCode">,
  read World81: EnumItem<"KeyCode">,
  read World82: EnumItem<"KeyCode">,
  read World83: EnumItem<"KeyCode">,
  read World84: EnumItem<"KeyCode">,
  read World85: EnumItem<"KeyCode">,
  read World86: EnumItem<"KeyCode">,
  read World87: EnumItem<"KeyCode">,
  read World88: EnumItem<"KeyCode">,
  read World89: EnumItem<"KeyCode">,
  read World90: EnumItem<"KeyCode">,
  read World91: EnumItem<"KeyCode">,
  read World92: EnumItem<"KeyCode">,
  read World93: EnumItem<"KeyCode">,
  read World94: EnumItem<"KeyCode">,
  read World95: EnumItem<"KeyCode">,
  read KeypadZero: EnumItem<"KeyCode">,
  read KeypadOne: EnumItem<"KeyCode">,
  read KeypadTwo: EnumItem<"KeyCode">,
  read KeypadThree: EnumItem<"KeyCode">,
  read KeypadFour: EnumItem<"KeyCode">,
  read KeypadFive: EnumItem<"KeyCode">,
  read KeypadSix: EnumItem<"KeyCode">,
  read KeypadSeven: EnumItem<"KeyCode">,
  read KeypadEight: EnumItem<"KeyCode">,
  read KeypadNine: EnumItem<"KeyCode">,
  read KeypadPeriod: EnumItem<"KeyCode">,
  read KeypadDivide: EnumItem<"KeyCode">,
  read KeypadMultiply: EnumItem<"KeyCode">,
  read KeypadMinus: EnumItem<"KeyCode">,
  read KeypadPlus: EnumItem<"KeyCode">,
  read KeypadEnter: EnumItem<"KeyCode">,
  read KeypadEquals: EnumItem<"KeyCode">,
  read Up: EnumItem<"KeyCode">,
  read Down: EnumItem<"KeyCode">,
  read Right: EnumItem<"KeyCode">,
  read Left: EnumItem<"KeyCode">,
  read Insert: EnumItem<"KeyCode">,
  read Home: EnumItem<"KeyCode">,
  read End: EnumItem<"KeyCode">,
  read PageUp: EnumItem<"KeyCode">,
  read PageDown: EnumItem<"KeyCode">,
  read F1: EnumItem<"KeyCode">,
  read F2: EnumItem<"KeyCode">,
  read F3: EnumItem<"KeyCode">,
  read F4: EnumItem<"KeyCode">,
  read F5: EnumItem<"KeyCode">,
  read F6: EnumItem<"KeyCode">,
  read F7: EnumItem<"KeyCode">,
  read F8: EnumItem<"KeyCode">,
  read F9: EnumItem<"KeyCode">,
  read F10: EnumItem<"KeyCode">,
  read F11: EnumItem<"KeyCode">,
  read F12: EnumItem<"KeyCode">,
  read F13: EnumItem<"KeyCode">,
  read F14: EnumItem<"KeyCode">,
  read F15: EnumItem<"KeyCode">,
  read NumLock: EnumItem<"KeyCode">,
  read CapsLock: EnumItem<"KeyCode">,
  read ScrollLock: EnumItem<"KeyCode">,
  read RightShift: EnumItem<"KeyCode">,
  read LeftShift: EnumItem<"KeyCode">,
  read RightControl: EnumItem<"KeyCode">,
  read LeftControl: EnumItem<"KeyCode">,
  read RightAlt: EnumItem<"KeyCode">,
  read LeftAlt: EnumItem<"KeyCode">,
  read RightMeta: EnumItem<"KeyCode">,
  read LeftMeta: EnumItem<"KeyCode">,
  read LeftSuper: EnumItem<"KeyCode">,
  read RightSuper: EnumItem<"KeyCode">,
  read Mode: EnumItem<"KeyCode">,
  read Compose: EnumItem<"KeyCode">,
  read Help: EnumItem<"KeyCode">,
  read Print: EnumItem<"KeyCode">,
  read SysReq: EnumItem<"KeyCode">,
  read Break: EnumItem<"KeyCode">,
  read Menu: EnumItem<"KeyCode">,
  read Power: EnumItem<"KeyCode">,
  read Euro: EnumItem<"KeyCode">,
  read Undo: EnumItem<"KeyCode">,
  read ButtonX: EnumItem<"KeyCode">,
  read ButtonY: EnumItem<"KeyCode">,
  read ButtonA: EnumItem<"KeyCode">,
  read ButtonB: EnumItem<"KeyCode">,
  read ButtonR1: EnumItem<"KeyCode">,
  read ButtonL1: EnumItem<"KeyCode">,
  read ButtonR2: EnumItem<"KeyCode">,
  read ButtonL2: EnumItem<"KeyCode">,
  read ButtonR3: EnumItem<"KeyCode">,
  read ButtonL3: EnumItem<"KeyCode">,
  read ButtonStart: EnumItem<"KeyCode">,
  read ButtonSelect: EnumItem<"KeyCode">,
  read DPadLeft: EnumItem<"KeyCode">,
  read DPadRight: EnumItem<"KeyCode">,
  read DPadUp: EnumItem<"KeyCode">,
  read DPadDown: EnumItem<"KeyCode">,
  read Thumbstick1: EnumItem<"KeyCode">,
  read Thumbstick2: EnumItem<"KeyCode">,
  read Thumbstick1Up: EnumItem<"KeyCode">,
  read Thumbstick1Down: EnumItem<"KeyCode">,
  read Thumbstick1Left: EnumItem<"KeyCode">,
  read Thumbstick1Right: EnumItem<"KeyCode">,
  read Thumbstick2Up: EnumItem<"KeyCode">,
  read Thumbstick2Down: EnumItem<"KeyCode">,
  read Thumbstick2Left: EnumItem<"KeyCode">,
  read Thumbstick2Right: EnumItem<"KeyCode">,
  read MouseLeftButton: EnumItem<"KeyCode">,
  read MouseRightButton: EnumItem<"KeyCode">,
  read MouseMiddleButton: EnumItem<"KeyCode">,
  read MouseBackButton: EnumItem<"KeyCode">,
  read MouseNoButton: EnumItem<"KeyCode">,
  read MouseX: EnumItem<"KeyCode">,
  read MouseY: EnumItem<"KeyCode">,
  read MousePosition: EnumItem<"KeyCode">,
  read TouchPosition: EnumItem<"KeyCode">,
  read MouseWheel: EnumItem<"KeyCode">,
  read TrackpadPan: EnumItem<"KeyCode">,
  read TrackpadPinch: EnumItem<"KeyCode">,
  read MouseDelta: EnumItem<"KeyCode">,
  read TouchDelta: EnumItem<"KeyCode">,
  read TouchPinch: EnumItem<"KeyCode">,
}
type KeyCodeStringFormat = {
  read Default: EnumItem<"KeyCodeStringFormat">,
  read Abbreviated: EnumItem<"KeyCodeStringFormat">,
}
type KeyInterpolationMode = {
  read Constant: EnumItem<"KeyInterpolationMode">,
  read Linear: EnumItem<"KeyInterpolationMode">,
  read Cubic: EnumItem<"KeyInterpolationMode">,
}
type KeywordFilterType = {
  read Include: EnumItem<"KeywordFilterType">,
  read Exclude: EnumItem<"KeywordFilterType">,
}
type Language = {
  read Default: EnumItem<"Language">,
}
type LeftRight = {
  read Left: EnumItem<"LeftRight">,
  read Center: EnumItem<"LeftRight">,
  read Right: EnumItem<"LeftRight">,
}
type LexemeType = {
  read Eof: EnumItem<"LexemeType">,
  read Name: EnumItem<"LexemeType">,
  read QuotedString: EnumItem<"LexemeType">,
  read Number: EnumItem<"LexemeType">,
  read And: EnumItem<"LexemeType">,
  read Or: EnumItem<"LexemeType">,
  read Equal: EnumItem<"LexemeType">,
  read TildeEqual: EnumItem<"LexemeType">,
  read GreaterThan: EnumItem<"LexemeType">,
  read GreaterThanEqual: EnumItem<"LexemeType">,
  read LessThan: EnumItem<"LexemeType">,
  read LessThanEqual: EnumItem<"LexemeType">,
  read Colon: EnumItem<"LexemeType">,
  read Dot: EnumItem<"LexemeType">,
  read LeftParenthesis: EnumItem<"LexemeType">,
  read RightParenthesis: EnumItem<"LexemeType">,
  read Star: EnumItem<"LexemeType">,
  read DoubleStar: EnumItem<"LexemeType">,
  read ReservedSpecial: EnumItem<"LexemeType">,
}
type LightingStyle = {
  read Realistic: EnumItem<"LightingStyle">,
  read Soft: EnumItem<"LightingStyle">,
}
type Limb = {
  read Head: EnumItem<"Limb">,
  read Torso: EnumItem<"Limb">,
  read LeftArm: EnumItem<"Limb">,
  read RightArm: EnumItem<"Limb">,
  read LeftLeg: EnumItem<"Limb">,
  read RightLeg: EnumItem<"Limb">,
  read Unknown: EnumItem<"Limb">,
}
type LineJoinMode = {
  read Round: EnumItem<"LineJoinMode">,
  read Bevel: EnumItem<"LineJoinMode">,
  read Miter: EnumItem<"LineJoinMode">,
}
type ListDisplayMode = {
  read Horizontal: EnumItem<"ListDisplayMode">,
  read Vertical: EnumItem<"ListDisplayMode">,
}
type ListenerLocation = {
  read Default: EnumItem<"ListenerLocation">,
  read None: EnumItem<"ListenerLocation">,
  read Character: EnumItem<"ListenerLocation">,
  read Camera: EnumItem<"ListenerLocation">,
}
type ListenerPositionType = {
  read Parent: EnumItem<"ListenerPositionType">,
  read Instance: EnumItem<"ListenerPositionType">,
}
type ListenerType = {
  read Camera: EnumItem<"ListenerType">,
  read CFrame: EnumItem<"ListenerType">,
  read ObjectPosition: EnumItem<"ListenerType">,
  read ObjectCFrame: EnumItem<"ListenerType">,
}
type LiveEditingAtomicUpdateResponse = {
  read Success: EnumItem<"LiveEditingAtomicUpdateResponse">,
  read FailureGuidNotFound: EnumItem<"LiveEditingAtomicUpdateResponse">,
  read FailureHashMismatch: EnumItem<"LiveEditingAtomicUpdateResponse">,
  read FailureOperationIllegal: EnumItem<"LiveEditingAtomicUpdateResponse">,
}
type LiveEditingBroadcastMessageType = {
  read Normal: EnumItem<"LiveEditingBroadcastMessageType">,
  read Warning: EnumItem<"LiveEditingBroadcastMessageType">,
  read Error: EnumItem<"LiveEditingBroadcastMessageType">,
}
type LoadCharacterLayeredClothing = {
  read Default: EnumItem<"LoadCharacterLayeredClothing">,
  read Disabled: EnumItem<"LoadCharacterLayeredClothing">,
  read Enabled: EnumItem<"LoadCharacterLayeredClothing">,
}
type LoadDynamicHeads = {
  read Default: EnumItem<"LoadDynamicHeads">,
  read Disabled: EnumItem<"LoadDynamicHeads">,
  read Enabled: EnumItem<"LoadDynamicHeads">,
}
type LocationType = {
  read Character: EnumItem<"LocationType">,
  read Camera: EnumItem<"LocationType">,
  read ObjectPosition: EnumItem<"LocationType">,
}
type LuauTypeCheckMode = {
  read Default: EnumItem<"LuauTypeCheckMode">,
  read NoCheck: EnumItem<"LuauTypeCheckMode">,
  read Nonstrict: EnumItem<"LuauTypeCheckMode">,
  read Strict: EnumItem<"LuauTypeCheckMode">,
}
type MakeupType = {
  read Face: EnumItem<"MakeupType">,
  read Lip: EnumItem<"MakeupType">,
  read Eye: EnumItem<"MakeupType">,
}
type MarketplaceBulkPurchasePromptStatus = {
  read Completed: EnumItem<"MarketplaceBulkPurchasePromptStatus">,
  read Aborted: EnumItem<"MarketplaceBulkPurchasePromptStatus">,
  read Error: EnumItem<"MarketplaceBulkPurchasePromptStatus">,
}
type MarketplaceItemPurchaseStatus = {
  read Success: EnumItem<"MarketplaceItemPurchaseStatus">,
  read SystemError: EnumItem<"MarketplaceItemPurchaseStatus">,
  read AlreadyOwned: EnumItem<"MarketplaceItemPurchaseStatus">,
  read InsufficientRobux: EnumItem<"MarketplaceItemPurchaseStatus">,
  read QuantityLimitExceeded: EnumItem<"MarketplaceItemPurchaseStatus">,
  read QuotaExceeded: EnumItem<"MarketplaceItemPurchaseStatus">,
  read NotForSale: EnumItem<"MarketplaceItemPurchaseStatus">,
  read NotAvailableForPurchaser: EnumItem<"MarketplaceItemPurchaseStatus">,
  read PriceMismatch: EnumItem<"MarketplaceItemPurchaseStatus">,
  read SoldOut: EnumItem<"MarketplaceItemPurchaseStatus">,
  read PurchaserIsSeller: EnumItem<"MarketplaceItemPurchaseStatus">,
  read InsufficientMembership: EnumItem<"MarketplaceItemPurchaseStatus">,
  read PlaceInvalid: EnumItem<"MarketplaceItemPurchaseStatus">,
}
type MarketplaceProductType = {
  read AvatarAsset: EnumItem<"MarketplaceProductType">,
  read AvatarBundle: EnumItem<"MarketplaceProductType">,
}
type MarkupKind = {
  read PlainText: EnumItem<"MarkupKind">,
  read Markdown: EnumItem<"MarkupKind">,
}
type MatchmakingType = {
  read Default: EnumItem<"MatchmakingType">,
  read XboxOnly: EnumItem<"MatchmakingType">,
  read PlayStationOnly: EnumItem<"MatchmakingType">,
}
type Material = {
  read Plastic: EnumItem<"Material">,
  read SmoothPlastic: EnumItem<"Material">,
  read Neon: EnumItem<"Material">,
  read Wood: EnumItem<"Material">,
  read WoodPlanks: EnumItem<"Material">,
  read Marble: EnumItem<"Material">,
  read Basalt: EnumItem<"Material">,
  read Slate: EnumItem<"Material">,
  read CrackedLava: EnumItem<"Material">,
  read Concrete: EnumItem<"Material">,
  read Limestone: EnumItem<"Material">,
  read Granite: EnumItem<"Material">,
  read Pavement: EnumItem<"Material">,
  read Brick: EnumItem<"Material">,
  read Pebble: EnumItem<"Material">,
  read Cobblestone: EnumItem<"Material">,
  read Rock: EnumItem<"Material">,
  read Sandstone: EnumItem<"Material">,
  read CorrodedMetal: EnumItem<"Material">,
  read DiamondPlate: EnumItem<"Material">,
  read Foil: EnumItem<"Material">,
  read Metal: EnumItem<"Material">,
  read Grass: EnumItem<"Material">,
  read LeafyGrass: EnumItem<"Material">,
  read Sand: EnumItem<"Material">,
  read Fabric: EnumItem<"Material">,
  read Snow: EnumItem<"Material">,
  read Mud: EnumItem<"Material">,
  read Ground: EnumItem<"Material">,
  read Asphalt: EnumItem<"Material">,
  read Salt: EnumItem<"Material">,
  read Ice: EnumItem<"Material">,
  read Glacier: EnumItem<"Material">,
  read Glass: EnumItem<"Material">,
  read ForceField: EnumItem<"Material">,
  read Air: EnumItem<"Material">,
  read Water: EnumItem<"Material">,
  read Cardboard: EnumItem<"Material">,
  read Carpet: EnumItem<"Material">,
  read CeramicTiles: EnumItem<"Material">,
  read ClayRoofTiles: EnumItem<"Material">,
  read RoofShingles: EnumItem<"Material">,
  read Leather: EnumItem<"Material">,
  read Plaster: EnumItem<"Material">,
  read Rubber: EnumItem<"Material">,
}
type MaterialPattern = {
  read Regular: EnumItem<"MaterialPattern">,
  read Organic: EnumItem<"MaterialPattern">,
}
type MembershipType = {
  read None: EnumItem<"MembershipType">,
  read BuildersClub: EnumItem<"MembershipType">,
  read TurboBuildersClub: EnumItem<"MembershipType">,
  read OutrageousBuildersClub: EnumItem<"MembershipType">,
  read Premium: EnumItem<"MembershipType">,
}
type MeshPartDetailLevel = {
  read DistanceBased: EnumItem<"MeshPartDetailLevel">,
  read Level00: EnumItem<"MeshPartDetailLevel">,
  read Level01: EnumItem<"MeshPartDetailLevel">,
  read Level02: EnumItem<"MeshPartDetailLevel">,
  read Level03: EnumItem<"MeshPartDetailLevel">,
  read Level04: EnumItem<"MeshPartDetailLevel">,
  read Level05: EnumItem<"MeshPartDetailLevel">,
  read Level06: EnumItem<"MeshPartDetailLevel">,
  read Level07: EnumItem<"MeshPartDetailLevel">,
  read Level08: EnumItem<"MeshPartDetailLevel">,
  read Level09: EnumItem<"MeshPartDetailLevel">,
}
type MeshPartHeadsAndAccessories = {
  read Default: EnumItem<"MeshPartHeadsAndAccessories">,
  read Disabled: EnumItem<"MeshPartHeadsAndAccessories">,
  read Enabled: EnumItem<"MeshPartHeadsAndAccessories">,
}
type MeshScaleUnit = {
  read Stud: EnumItem<"MeshScaleUnit">,
  read Meter: EnumItem<"MeshScaleUnit">,
  read CM: EnumItem<"MeshScaleUnit">,
  read MM: EnumItem<"MeshScaleUnit">,
  read Foot: EnumItem<"MeshScaleUnit">,
  read Inch: EnumItem<"MeshScaleUnit">,
}
type MeshType = {
  read Head: EnumItem<"MeshType">,
  read Torso: EnumItem<"MeshType">,
  read Wedge: EnumItem<"MeshType">,
  read Sphere: EnumItem<"MeshType">,
  read Cylinder: EnumItem<"MeshType">,
  read FileMesh: EnumItem<"MeshType">,
  read Brick: EnumItem<"MeshType">,
  read Prism: EnumItem<"MeshType">,
  read Pyramid: EnumItem<"MeshType">,
  read ParallelRamp: EnumItem<"MeshType">,
  read RightAngleRamp: EnumItem<"MeshType">,
  read CornerWedge: EnumItem<"MeshType">,
}
type MessageType = {
  read MessageOutput: EnumItem<"MessageType">,
  read MessageInfo: EnumItem<"MessageType">,
  read MessageWarning: EnumItem<"MessageType">,
  read MessageError: EnumItem<"MessageType">,
}
type ModelLevelOfDetail = {
  read Automatic: EnumItem<"ModelLevelOfDetail">,
  read StreamingMesh: EnumItem<"ModelLevelOfDetail">,
  read Disabled: EnumItem<"ModelLevelOfDetail">,
  read SLIM: EnumItem<"ModelLevelOfDetail">,
}
type ModelStreamingBehavior = {
  read Default: EnumItem<"ModelStreamingBehavior">,
  read Legacy: EnumItem<"ModelStreamingBehavior">,
  read Improved: EnumItem<"ModelStreamingBehavior">,
}
type ModelStreamingMode = {
  read Default: EnumItem<"ModelStreamingMode">,
  read Atomic: EnumItem<"ModelStreamingMode">,
  read Persistent: EnumItem<"ModelStreamingMode">,
  read PersistentPerPlayer: EnumItem<"ModelStreamingMode">,
  read Nonatomic: EnumItem<"ModelStreamingMode">,
}
type ModerationResultCategory = {
  read ViolationDetected: EnumItem<"ModerationResultCategory">,
  read Borderline: EnumItem<"ModerationResultCategory">,
  read NoViolationDetected: EnumItem<"ModerationResultCategory">,
}
type ModerationResultLabel = {
  read ChildExploitation: EnumItem<"ModerationResultLabel">,
  read SuicideSelfInjuryAndHarmfulBehavior: EnumItem<"ModerationResultLabel">,
  read ThreatsBullyingAndHarassment: EnumItem<"ModerationResultLabel">,
  read TerrorismAndViolentExtremism: EnumItem<"ModerationResultLabel">,
  read DiscriminationSlursAndHateSpeech: EnumItem<"ModerationResultLabel">,
  read RealWorldSensitiveEvents: EnumItem<"ModerationResultLabel">,
  read ViolentContentAndGore: EnumItem<"ModerationResultLabel">,
  read RomanticAndSexualContent: EnumItem<"ModerationResultLabel">,
  read IllegalAndRegulatedGoodsAndActivities: EnumItem<"ModerationResultLabel">,
  read Profanity: EnumItem<"ModerationResultLabel">,
  read Other: EnumItem<"ModerationResultLabel">,
}
type ModerationStatus = {
  read ReviewedApproved: EnumItem<"ModerationStatus">,
  read ReviewedRejected: EnumItem<"ModerationStatus">,
  read NotReviewed: EnumItem<"ModerationStatus">,
  read NotApplicable: EnumItem<"ModerationStatus">,
  read Invalid: EnumItem<"ModerationStatus">,
}
type ModifierKey = {
  read Shift: EnumItem<"ModifierKey">,
  read Ctrl: EnumItem<"ModifierKey">,
  read Alt: EnumItem<"ModifierKey">,
  read Meta: EnumItem<"ModifierKey">,
}
type MouseBehavior = {
  read Default: EnumItem<"MouseBehavior">,
  read LockCenter: EnumItem<"MouseBehavior">,
  read LockCurrentPosition: EnumItem<"MouseBehavior">,
}
type MoveState = {
  read Stopped: EnumItem<"MoveState">,
  read Coasting: EnumItem<"MoveState">,
  read Pushing: EnumItem<"MoveState">,
  read Stopping: EnumItem<"MoveState">,
  read AirFree: EnumItem<"MoveState">,
}
type MuteState = {
  read Unmuted: EnumItem<"MuteState">,
  read Muted: EnumItem<"MuteState">,
}
type NameOcclusion = {
  read NoOcclusion: EnumItem<"NameOcclusion">,
  read EnemyOcclusion: EnumItem<"NameOcclusion">,
  read OccludeAll: EnumItem<"NameOcclusion">,
}
type NegateOperationHiddenHistory = {
  read None: EnumItem<"NegateOperationHiddenHistory">,
  read NegatedUnion: EnumItem<"NegateOperationHiddenHistory">,
  read NegatedIntersection: EnumItem<"NegateOperationHiddenHistory">,
}
type NetworkOwnership = {
  read Automatic: EnumItem<"NetworkOwnership">,
  read Manual: EnumItem<"NetworkOwnership">,
  read OnContact: EnumItem<"NetworkOwnership">,
}
type NetworkStatus = {
  read Unknown: EnumItem<"NetworkStatus">,
  read Connected: EnumItem<"NetworkStatus">,
  read Disconnected: EnumItem<"NetworkStatus">,
}
type NoiseType = {
  read SimplexGabor: EnumItem<"NoiseType">,
}
type NormalId = {
  read Right: EnumItem<"NormalId">,
  read Top: EnumItem<"NormalId">,
  read Back: EnumItem<"NormalId">,
  read Left: EnumItem<"NormalId">,
  read Bottom: EnumItem<"NormalId">,
  read Front: EnumItem<"NormalId">,
}
type NotificationButtonType = {
  read Primary: EnumItem<"NotificationButtonType">,
  read Secondary: EnumItem<"NotificationButtonType">,
}
type OperationType = {
  read Null: EnumItem<"OperationType">,
  read Union: EnumItem<"OperationType">,
  read Subtraction: EnumItem<"OperationType">,
  read Intersection: EnumItem<"OperationType">,
  read Primitive: EnumItem<"OperationType">,
}
type OrientationAlignmentMode = {
  read OneAttachment: EnumItem<"OrientationAlignmentMode">,
  read TwoAttachment: EnumItem<"OrientationAlignmentMode">,
}
type OutfitSource = {
  read All: EnumItem<"OutfitSource">,
  read Created: EnumItem<"OutfitSource">,
  read Purchased: EnumItem<"OutfitSource">,
}
type OutfitType = {
  read All: EnumItem<"OutfitType">,
  read Avatar: EnumItem<"OutfitType">,
  read DynamicHead: EnumItem<"OutfitType">,
  read Shoes: EnumItem<"OutfitType">,
  read Makeup: EnumItem<"OutfitType">,
}
type OutputLayoutMode = {
  read Horizontal: EnumItem<"OutputLayoutMode">,
  read Vertical: EnumItem<"OutputLayoutMode">,
}
type OverrideMouseIconBehavior = {
  read None: EnumItem<"OverrideMouseIconBehavior">,
  read ForceShow: EnumItem<"OverrideMouseIconBehavior">,
  read ForceHide: EnumItem<"OverrideMouseIconBehavior">,
}
type PackagePermission = {
  read None: EnumItem<"PackagePermission">,
  read NoAccess: EnumItem<"PackagePermission">,
  read Revoked: EnumItem<"PackagePermission">,
  read UseView: EnumItem<"PackagePermission">,
  read Edit: EnumItem<"PackagePermission">,
  read Own: EnumItem<"PackagePermission">,
}
type PartType = {
  read Ball: EnumItem<"PartType">,
  read Block: EnumItem<"PartType">,
  read Cylinder: EnumItem<"PartType">,
  read Wedge: EnumItem<"PartType">,
  read CornerWedge: EnumItem<"PartType">,
}
type ParticleEmitterShape = {
  read Box: EnumItem<"ParticleEmitterShape">,
  read Sphere: EnumItem<"ParticleEmitterShape">,
  read Cylinder: EnumItem<"ParticleEmitterShape">,
  read Disc: EnumItem<"ParticleEmitterShape">,
}
type ParticleEmitterShapeInOut = {
  read Outward: EnumItem<"ParticleEmitterShapeInOut">,
  read Inward: EnumItem<"ParticleEmitterShapeInOut">,
  read InAndOut: EnumItem<"ParticleEmitterShapeInOut">,
}
type ParticleEmitterShapeStyle = {
  read Volume: EnumItem<"ParticleEmitterShapeStyle">,
  read Surface: EnumItem<"ParticleEmitterShapeStyle">,
}
type ParticleFlipbookLayout = {
  read None: EnumItem<"ParticleFlipbookLayout">,
  read Grid2x2: EnumItem<"ParticleFlipbookLayout">,
  read Grid4x4: EnumItem<"ParticleFlipbookLayout">,
  read Grid8x8: EnumItem<"ParticleFlipbookLayout">,
  read Custom: EnumItem<"ParticleFlipbookLayout">,
}
type ParticleFlipbookMode = {
  read Loop: EnumItem<"ParticleFlipbookMode">,
  read OneShot: EnumItem<"ParticleFlipbookMode">,
  read PingPong: EnumItem<"ParticleFlipbookMode">,
  read Random: EnumItem<"ParticleFlipbookMode">,
}
type ParticleFlipbookTextureCompatible = {
  read NotCompatible: EnumItem<"ParticleFlipbookTextureCompatible">,
  read Compatible: EnumItem<"ParticleFlipbookTextureCompatible">,
  read Unknown: EnumItem<"ParticleFlipbookTextureCompatible">,
}
type ParticleOrientation = {
  read FacingCamera: EnumItem<"ParticleOrientation">,
  read FacingCameraWorldUp: EnumItem<"ParticleOrientation">,
  read VelocityParallel: EnumItem<"ParticleOrientation">,
  read VelocityPerpendicular: EnumItem<"ParticleOrientation">,
}
type PathStatus = {
  read Success: EnumItem<"PathStatus">,
  read ClosestNoPath: EnumItem<"PathStatus">,
  read ClosestOutOfRange: EnumItem<"PathStatus">,
  read FailStartNotEmpty: EnumItem<"PathStatus">,
  read FailFinishNotEmpty: EnumItem<"PathStatus">,
  read NoPath: EnumItem<"PathStatus">,
}
type PathWaypointAction = {
  read Walk: EnumItem<"PathWaypointAction">,
  read Jump: EnumItem<"PathWaypointAction">,
  read Custom: EnumItem<"PathWaypointAction">,
}
type PathfindingUseImprovedSearch = {
  read Default: EnumItem<"PathfindingUseImprovedSearch">,
  read Disabled: EnumItem<"PathfindingUseImprovedSearch">,
  read Enabled: EnumItem<"PathfindingUseImprovedSearch">,
}
type PeoplePageLayout = {
  read Card: EnumItem<"PeoplePageLayout">,
  read List: EnumItem<"PeoplePageLayout">,
}
type PerformanceOverlayMode = {
  read Overdraw: EnumItem<"PerformanceOverlayMode">,
  read Transparent: EnumItem<"PerformanceOverlayMode">,
  read Decals: EnumItem<"PerformanceOverlayMode">,
  read Lights: EnumItem<"PerformanceOverlayMode">,
}
type PermissionLevelShown = {
  read Game: EnumItem<"PermissionLevelShown">,
  read RobloxGame: EnumItem<"PermissionLevelShown">,
  read RobloxScript: EnumItem<"PermissionLevelShown">,
  read Studio: EnumItem<"PermissionLevelShown">,
  read Roblox: EnumItem<"PermissionLevelShown">,
}
type PhysicalConstraintType = {
  read AnimationConstraint: EnumItem<"PhysicalConstraintType">,
  read Motor6D: EnumItem<"PhysicalConstraintType">,
}
type PhysicsSimulationRate = {
  read Fixed240Hz: EnumItem<"PhysicsSimulationRate">,
  read Fixed120Hz: EnumItem<"PhysicsSimulationRate">,
  read Fixed60Hz: EnumItem<"PhysicsSimulationRate">,
}
type PhysicsSteppingMethod = {
  read Default: EnumItem<"PhysicsSteppingMethod">,
  read Fixed: EnumItem<"PhysicsSteppingMethod">,
  read Adaptive: EnumItem<"PhysicsSteppingMethod">,
}
type PlaceContentPreference = {
  read None: EnumItem<"PlaceContentPreference">,
  read All: EnumItem<"PlaceContentPreference">,
  read MentionsAndReplies: EnumItem<"PlaceContentPreference">,
  read Unknown: EnumItem<"PlaceContentPreference">,
}
type PlacePublishType = {
  read None: EnumItem<"PlacePublishType">,
  read Publish: EnumItem<"PlacePublishType">,
  read Save: EnumItem<"PlacePublishType">,
}
type Platform = {
  read Windows: EnumItem<"Platform">,
  read OSX: EnumItem<"Platform">,
  read IOS: EnumItem<"Platform">,
  read Android: EnumItem<"Platform">,
  read XBoxOne: EnumItem<"Platform">,
  read PS4: EnumItem<"Platform">,
  read PS3: EnumItem<"Platform">,
  read XBox360: EnumItem<"Platform">,
  read WiiU: EnumItem<"Platform">,
  read NX: EnumItem<"Platform">,
  read Ouya: EnumItem<"Platform">,
  read AndroidTV: EnumItem<"Platform">,
  read Chromecast: EnumItem<"Platform">,
  read Linux: EnumItem<"Platform">,
  read SteamOS: EnumItem<"Platform">,
  read WebOS: EnumItem<"Platform">,
  read DOS: EnumItem<"Platform">,
  read BeOS: EnumItem<"Platform">,
  read UWP: EnumItem<"Platform">,
  read PS5: EnumItem<"Platform">,
  read MetaOS: EnumItem<"Platform">,
  read Web: EnumItem<"Platform">,
  read None: EnumItem<"Platform">,
}
type PlaybackState = {
  read Begin: EnumItem<"PlaybackState">,
  read Delayed: EnumItem<"PlaybackState">,
  read Playing: EnumItem<"PlaybackState">,
  read Paused: EnumItem<"PlaybackState">,
  read Completed: EnumItem<"PlaybackState">,
  read Cancelled: EnumItem<"PlaybackState">,
}
type PlayerActions = {
  read CharacterForward: EnumItem<"PlayerActions">,
  read CharacterBackward: EnumItem<"PlayerActions">,
  read CharacterLeft: EnumItem<"PlayerActions">,
  read CharacterRight: EnumItem<"PlayerActions">,
  read CharacterJump: EnumItem<"PlayerActions">,
}
type PlayerCharacterDestroyBehavior = {
  read Default: EnumItem<"PlayerCharacterDestroyBehavior">,
  read Disabled: EnumItem<"PlayerCharacterDestroyBehavior">,
  read Enabled: EnumItem<"PlayerCharacterDestroyBehavior">,
}
type PlayerChatType = {
  read All: EnumItem<"PlayerChatType">,
  read Team: EnumItem<"PlayerChatType">,
  read Whisper: EnumItem<"PlayerChatType">,
}
type PlayerDataErrorState = {
  read LoadFailed: EnumItem<"PlayerDataErrorState">,
  read FlushFailed: EnumItem<"PlayerDataErrorState">,
  read ReleaseFailed: EnumItem<"PlayerDataErrorState">,
  read None: EnumItem<"PlayerDataErrorState">,
}
type PlayerDataLoadFailureBehavior = {
  read Failure: EnumItem<"PlayerDataLoadFailureBehavior">,
  read FallbackToDefault: EnumItem<"PlayerDataLoadFailureBehavior">,
  read Kick: EnumItem<"PlayerDataLoadFailureBehavior">,
}
type PlayerExitReason = {
  read Unknown: EnumItem<"PlayerExitReason">,
  read PlatformKick: EnumItem<"PlayerExitReason">,
  read CreatorKick: EnumItem<"PlayerExitReason">,
}
type PlayerPlatformActivationStatus = {
  read Unknown: EnumItem<"PlayerPlatformActivationStatus">,
  read New: EnumItem<"PlayerPlatformActivationStatus">,
  read Active: EnumItem<"PlayerPlatformActivationStatus">,
  read Lapsed: EnumItem<"PlayerPlatformActivationStatus">,
  read Reactivated: EnumItem<"PlayerPlatformActivationStatus">,
}
type PlayerPlatformSpenderStatus = {
  read Unknown: EnumItem<"PlayerPlatformSpenderStatus">,
  read Active: EnumItem<"PlayerPlatformSpenderStatus">,
  read OtherPayer: EnumItem<"PlayerPlatformSpenderStatus">,
}
type PluginConnectionTargetType = {
  read Edit: EnumItem<"PluginConnectionTargetType">,
  read Test: EnumItem<"PluginConnectionTargetType">,
}
type PoseEasingDirection = {
  read In: EnumItem<"PoseEasingDirection">,
  read Out: EnumItem<"PoseEasingDirection">,
  read InOut: EnumItem<"PoseEasingDirection">,
}
type PoseEasingStyle = {
  read Linear: EnumItem<"PoseEasingStyle">,
  read Constant: EnumItem<"PoseEasingStyle">,
  read Elastic: EnumItem<"PoseEasingStyle">,
  read Cubic: EnumItem<"PoseEasingStyle">,
  read Bounce: EnumItem<"PoseEasingStyle">,
  read CubicV2: EnumItem<"PoseEasingStyle">,
}
type PositionAlignmentMode = {
  read OneAttachment: EnumItem<"PositionAlignmentMode">,
  read TwoAttachment: EnumItem<"PositionAlignmentMode">,
}
type PredictionMode = {
  read Automatic: EnumItem<"PredictionMode">,
  read On: EnumItem<"PredictionMode">,
  read Off: EnumItem<"PredictionMode">,
}
type PredictionStatus = {
  read Authoritative: EnumItem<"PredictionStatus">,
  read Predicted: EnumItem<"PredictionStatus">,
  read None: EnumItem<"PredictionStatus">,
}
type PredictiveStreamingMode = {
  read Default: EnumItem<"PredictiveStreamingMode">,
  read Enabled: EnumItem<"PredictiveStreamingMode">,
  read Disabled: EnumItem<"PredictiveStreamingMode">,
}
type PreferredInput = {
  read KeyboardAndMouse: EnumItem<"PreferredInput">,
  read Gamepad: EnumItem<"PreferredInput">,
  read Touch: EnumItem<"PreferredInput">,
}
type PreferredTextSize = {
  read Medium: EnumItem<"PreferredTextSize">,
  read Large: EnumItem<"PreferredTextSize">,
  read Larger: EnumItem<"PreferredTextSize">,
  read Largest: EnumItem<"PreferredTextSize">,
}
type PrefetchDownloadStatus = {
  read NotStarted: EnumItem<"PrefetchDownloadStatus">,
  read InProgress: EnumItem<"PrefetchDownloadStatus">,
  read Completed: EnumItem<"PrefetchDownloadStatus">,
  read Failed: EnumItem<"PrefetchDownloadStatus">,
}
type PrimalPhysicsSolver = {
  read Default: EnumItem<"PrimalPhysicsSolver">,
  read Experimental: EnumItem<"PrimalPhysicsSolver">,
  read Disabled: EnumItem<"PrimalPhysicsSolver">,
}
type PrimitiveType = {
  read Null: EnumItem<"PrimitiveType">,
  read Ball: EnumItem<"PrimitiveType">,
  read Cylinder: EnumItem<"PrimitiveType">,
  read Block: EnumItem<"PrimitiveType">,
  read Wedge: EnumItem<"PrimitiveType">,
  read CornerWedge: EnumItem<"PrimitiveType">,
}
type PrivilegeType = {
  read Banned: EnumItem<"PrivilegeType">,
  read Visitor: EnumItem<"PrivilegeType">,
  read Member: EnumItem<"PrivilegeType">,
  read Admin: EnumItem<"PrivilegeType">,
  read Owner: EnumItem<"PrivilegeType">,
}
type ProductLocationRestriction = {
  read AvatarShop: EnumItem<"ProductLocationRestriction">,
  read AllowedGames: EnumItem<"ProductLocationRestriction">,
  read AllGames: EnumItem<"ProductLocationRestriction">,
}
type ProductPurchaseChannel = {
  read InExperience: EnumItem<"ProductPurchaseChannel">,
  read ExperienceDetailsPage: EnumItem<"ProductPurchaseChannel">,
  read AdReward: EnumItem<"ProductPurchaseChannel">,
  read CommerceProduct: EnumItem<"ProductPurchaseChannel">,
}
type ProductPurchaseDecision = {
  read NotProcessedYet: EnumItem<"ProductPurchaseDecision">,
  read PurchaseGranted: EnumItem<"ProductPurchaseDecision">,
}
type PromptCreateAssetResult = {
  read Success: EnumItem<"PromptCreateAssetResult">,
  read PermissionDenied: EnumItem<"PromptCreateAssetResult">,
  read Timeout: EnumItem<"PromptCreateAssetResult">,
  read UploadFailed: EnumItem<"PromptCreateAssetResult">,
  read NoUserInput: EnumItem<"PromptCreateAssetResult">,
  read UnknownFailure: EnumItem<"PromptCreateAssetResult">,
  read UGCValidationFailed: EnumItem<"PromptCreateAssetResult">,
  read ModeratedName: EnumItem<"PromptCreateAssetResult">,
  read PurchaseFailure: EnumItem<"PromptCreateAssetResult">,
  read TokenInvalid: EnumItem<"PromptCreateAssetResult">,
}
type PromptCreateAvatarResult = {
  read Success: EnumItem<"PromptCreateAvatarResult">,
  read PermissionDenied: EnumItem<"PromptCreateAvatarResult">,
  read Timeout: EnumItem<"PromptCreateAvatarResult">,
  read UploadFailed: EnumItem<"PromptCreateAvatarResult">,
  read NoUserInput: EnumItem<"PromptCreateAvatarResult">,
  read InvalidHumanoidDescription: EnumItem<"PromptCreateAvatarResult">,
  read UGCValidationFailed: EnumItem<"PromptCreateAvatarResult">,
  read ModeratedName: EnumItem<"PromptCreateAvatarResult">,
  read MaxOutfits: EnumItem<"PromptCreateAvatarResult">,
  read PurchaseFailure: EnumItem<"PromptCreateAvatarResult">,
  read UnknownFailure: EnumItem<"PromptCreateAvatarResult">,
  read TokenInvalid: EnumItem<"PromptCreateAvatarResult">,
}
type PromptCreateOutfitResult = {
  read Success: EnumItem<"PromptCreateOutfitResult">,
  read PartialSuccess: EnumItem<"PromptCreateOutfitResult">,
  read CreationFailure: EnumItem<"PromptCreateOutfitResult">,
  read PermissionDenied: EnumItem<"PromptCreateOutfitResult">,
  read Timeout: EnumItem<"PromptCreateOutfitResult">,
  read NoUserInput: EnumItem<"PromptCreateOutfitResult">,
  read UnknownFailure: EnumItem<"PromptCreateOutfitResult">,
}
type PromptExperienceDetailsResult = {
  read PromptClosed: EnumItem<"PromptExperienceDetailsResult">,
  read TeleportAttempted: EnumItem<"PromptExperienceDetailsResult">,
}
type PromptLinkSharingResult = {
  read Success: EnumItem<"PromptLinkSharingResult">,
  read PlayerLeft: EnumItem<"PromptLinkSharingResult">,
  read InvalidLaunchData: EnumItem<"PromptLinkSharingResult">,
}
type PromptPublishAssetResult = {
  read Success: EnumItem<"PromptPublishAssetResult">,
  read PermissionDenied: EnumItem<"PromptPublishAssetResult">,
  read Timeout: EnumItem<"PromptPublishAssetResult">,
  read UploadFailed: EnumItem<"PromptPublishAssetResult">,
  read NoUserInput: EnumItem<"PromptPublishAssetResult">,
  read UnknownFailure: EnumItem<"PromptPublishAssetResult">,
}
type PropertyStatus = {
  read Ok: EnumItem<"PropertyStatus">,
  read Warning: EnumItem<"PropertyStatus">,
  read Error: EnumItem<"PropertyStatus">,
}
type ProximityPromptExclusivity = {
  read OnePerButton: EnumItem<"ProximityPromptExclusivity">,
  read OneGlobally: EnumItem<"ProximityPromptExclusivity">,
  read AlwaysShow: EnumItem<"ProximityPromptExclusivity">,
}
type ProximityPromptInputType = {
  read Keyboard: EnumItem<"ProximityPromptInputType">,
  read Gamepad: EnumItem<"ProximityPromptInputType">,
  read Touch: EnumItem<"ProximityPromptInputType">,
}
type ProximityPromptStyle = {
  read Default: EnumItem<"ProximityPromptStyle">,
  read Custom: EnumItem<"ProximityPromptStyle">,
}
type PurchaseOption = {
  read TimedOption: EnumItem<"PurchaseOption">,
  read Permanent: EnumItem<"PurchaseOption">,
}
type QualityLevel = {
  read Automatic: EnumItem<"QualityLevel">,
  read Level01: EnumItem<"QualityLevel">,
  read Level02: EnumItem<"QualityLevel">,
  read Level03: EnumItem<"QualityLevel">,
  read Level04: EnumItem<"QualityLevel">,
  read Level05: EnumItem<"QualityLevel">,
  read Level06: EnumItem<"QualityLevel">,
  read Level07: EnumItem<"QualityLevel">,
  read Level08: EnumItem<"QualityLevel">,
  read Level09: EnumItem<"QualityLevel">,
  read Level10: EnumItem<"QualityLevel">,
  read Level11: EnumItem<"QualityLevel">,
  read Level12: EnumItem<"QualityLevel">,
  read Level13: EnumItem<"QualityLevel">,
  read Level14: EnumItem<"QualityLevel">,
  read Level15: EnumItem<"QualityLevel">,
  read Level16: EnumItem<"QualityLevel">,
  read Level17: EnumItem<"QualityLevel">,
  read Level18: EnumItem<"QualityLevel">,
  read Level19: EnumItem<"QualityLevel">,
  read Level20: EnumItem<"QualityLevel">,
  read Level21: EnumItem<"QualityLevel">,
}
type R15CollisionType = {
  read OuterBox: EnumItem<"R15CollisionType">,
  read InnerBox: EnumItem<"R15CollisionType">,
}
type RaycastFilterType = {
  read Exclude: EnumItem<"RaycastFilterType">,
  read Include: EnumItem<"RaycastFilterType">,
}
type ReadCapturesFromGalleryResult = {
  read Success: EnumItem<"ReadCapturesFromGalleryResult">,
  read NeedPermission: EnumItem<"ReadCapturesFromGalleryResult">,
}
type ReceiptDecision = {
  read NotProcessedYet: EnumItem<"ReceiptDecision">,
  read Processed: EnumItem<"ReceiptDecision">,
}
type ReceiptType = {
  read DeveloperProduct: EnumItem<"ReceiptType">,
  read RobuxTransferSender: EnumItem<"ReceiptType">,
  read RobuxTransferReceiver: EnumItem<"ReceiptType">,
}
type RecommendationActionType = {
  read AddReaction: EnumItem<"RecommendationActionType">,
  read RemoveReaction: EnumItem<"RecommendationActionType">,
  read Share: EnumItem<"RecommendationActionType">,
  read Report: EnumItem<"RecommendationActionType">,
  read Comment: EnumItem<"RecommendationActionType">,
  read Play: EnumItem<"RecommendationActionType">,
  read Purchase: EnumItem<"RecommendationActionType">,
}
type RecommendationDepartureIntent = {
  read Neutral: EnumItem<"RecommendationDepartureIntent">,
  read Positive: EnumItem<"RecommendationDepartureIntent">,
  read Negative: EnumItem<"RecommendationDepartureIntent">,
}
type RecommendationImpressionType = {
  read View: EnumItem<"RecommendationImpressionType">,
  read NotViewable: EnumItem<"RecommendationImpressionType">,
}
type RecommendationItemContentType = {
  read Static: EnumItem<"RecommendationItemContentType">,
  read Dynamic: EnumItem<"RecommendationItemContentType">,
  read Interactive: EnumItem<"RecommendationItemContentType">,
}
type RecommendationItemVisibility = {
  read Private: EnumItem<"RecommendationItemVisibility">,
  read Public: EnumItem<"RecommendationItemVisibility">,
}
type RecommendationPreferenceTargetType = {
  read User: EnumItem<"RecommendationPreferenceTargetType">,
  read Universe: EnumItem<"RecommendationPreferenceTargetType">,
  read CustomTag: EnumItem<"RecommendationPreferenceTargetType">,
}
type RecommendationPreferenceType = {
  read AddFollow: EnumItem<"RecommendationPreferenceType">,
  read RemoveFollow: EnumItem<"RecommendationPreferenceType">,
  read AddMute: EnumItem<"RecommendationPreferenceType">,
  read RemoveMute: EnumItem<"RecommendationPreferenceType">,
}
type RejectCharacterDeletions = {
  read Default: EnumItem<"RejectCharacterDeletions">,
  read Disabled: EnumItem<"RejectCharacterDeletions">,
  read Enabled: EnumItem<"RejectCharacterDeletions">,
}
type RenderFidelity = {
  read Automatic: EnumItem<"RenderFidelity">,
  read Precise: EnumItem<"RenderFidelity">,
  read Performance: EnumItem<"RenderFidelity">,
}
type RenderPriority = {
  read First: EnumItem<"RenderPriority">,
  read Input: EnumItem<"RenderPriority">,
  read Camera: EnumItem<"RenderPriority">,
  read Character: EnumItem<"RenderPriority">,
  read Last: EnumItem<"RenderPriority">,
}
type RenderingCacheOptimizationMode = {
  read Default: EnumItem<"RenderingCacheOptimizationMode">,
  read Disabled: EnumItem<"RenderingCacheOptimizationMode">,
  read Enabled: EnumItem<"RenderingCacheOptimizationMode">,
}
type RenderingTestComparisonMethod = {
  read psnr: EnumItem<"RenderingTestComparisonMethod">,
  read diff: EnumItem<"RenderingTestComparisonMethod">,
}
type ReplicateInstanceDestroySetting = {
  read Default: EnumItem<"ReplicateInstanceDestroySetting">,
  read Disabled: EnumItem<"ReplicateInstanceDestroySetting">,
  read Enabled: EnumItem<"ReplicateInstanceDestroySetting">,
}
type ResamplerMode = {
  read Default: EnumItem<"ResamplerMode">,
  read Pixelated: EnumItem<"ResamplerMode">,
}
type ReservedHighlightId = {
  read Standard: EnumItem<"ReservedHighlightId">,
  read Active: EnumItem<"ReservedHighlightId">,
  read Hover: EnumItem<"ReservedHighlightId">,
  read Selection: EnumItem<"ReservedHighlightId">,
  read NegatedPart: EnumItem<"ReservedHighlightId">,
}
type RestPose = {
  read Default: EnumItem<"RestPose">,
  read RotationsReset: EnumItem<"RestPose">,
  read Custom: EnumItem<"RestPose">,
}
type RestPoseModel = {
  read FromRigInACE: EnumItem<"RestPoseModel">,
  read FromRigInFile: EnumItem<"RestPoseModel">,
  read FromRigInFileZeroedRotations: EnumItem<"RestPoseModel">,
  read FromCustomClip: EnumItem<"RestPoseModel">,
}
type ReturnKeyType = {
  read Default: EnumItem<"ReturnKeyType">,
  read Done: EnumItem<"ReturnKeyType">,
  read Go: EnumItem<"ReturnKeyType">,
  read Next: EnumItem<"ReturnKeyType">,
  read Search: EnumItem<"ReturnKeyType">,
  read Send: EnumItem<"ReturnKeyType">,
}
type ReverbType = {
  read NoReverb: EnumItem<"ReverbType">,
  read GenericReverb: EnumItem<"ReverbType">,
  read PaddedCell: EnumItem<"ReverbType">,
  read Room: EnumItem<"ReverbType">,
  read Bathroom: EnumItem<"ReverbType">,
  read LivingRoom: EnumItem<"ReverbType">,
  read StoneRoom: EnumItem<"ReverbType">,
  read Auditorium: EnumItem<"ReverbType">,
  read ConcertHall: EnumItem<"ReverbType">,
  read Cave: EnumItem<"ReverbType">,
  read Arena: EnumItem<"ReverbType">,
  read Hangar: EnumItem<"ReverbType">,
  read CarpettedHallway: EnumItem<"ReverbType">,
  read Hallway: EnumItem<"ReverbType">,
  read StoneCorridor: EnumItem<"ReverbType">,
  read Alley: EnumItem<"ReverbType">,
  read Forest: EnumItem<"ReverbType">,
  read City: EnumItem<"ReverbType">,
  read Mountains: EnumItem<"ReverbType">,
  read Quarry: EnumItem<"ReverbType">,
  read Plain: EnumItem<"ReverbType">,
  read ParkingLot: EnumItem<"ReverbType">,
  read SewerPipe: EnumItem<"ReverbType">,
  read UnderWater: EnumItem<"ReverbType">,
}
type ReviewableContentState = {
  read Pending: EnumItem<"ReviewableContentState">,
  read Completed: EnumItem<"ReviewableContentState">,
  read Failed: EnumItem<"ReviewableContentState">,
}
type RibbonTool = {
  read Select: EnumItem<"RibbonTool">,
  read Scale: EnumItem<"RibbonTool">,
  read Rotate: EnumItem<"RibbonTool">,
  read Move: EnumItem<"RibbonTool">,
  read Transform: EnumItem<"RibbonTool">,
  read ColorPicker: EnumItem<"RibbonTool">,
  read MaterialPicker: EnumItem<"RibbonTool">,
  read Group: EnumItem<"RibbonTool">,
  read Ungroup: EnumItem<"RibbonTool">,
  read None: EnumItem<"RibbonTool">,
  read PivotEditor: EnumItem<"RibbonTool">,
}
type RigLabel = {
  read Invalid: EnumItem<"RigLabel">,
  read Root: EnumItem<"RigLabel">,
  read LeftHip: EnumItem<"RigLabel">,
  read LeftKnee: EnumItem<"RigLabel">,
  read LeftAnkle: EnumItem<"RigLabel">,
  read RightHip: EnumItem<"RigLabel">,
  read RightKnee: EnumItem<"RigLabel">,
  read RightAnkle: EnumItem<"RigLabel">,
  read Waist: EnumItem<"RigLabel">,
  read LeftShoulder: EnumItem<"RigLabel">,
  read LeftElbow: EnumItem<"RigLabel">,
  read LeftWrist: EnumItem<"RigLabel">,
  read RightShoulder: EnumItem<"RigLabel">,
  read RightElbow: EnumItem<"RigLabel">,
  read RightWrist: EnumItem<"RigLabel">,
  read Neck: EnumItem<"RigLabel">,
  read Spine: EnumItem<"RigLabel">,
  read Chest: EnumItem<"RigLabel">,
  read HeadBase: EnumItem<"RigLabel">,
  read LeftClavicle: EnumItem<"RigLabel">,
  read RightClavicle: EnumItem<"RigLabel">,
  read LeftToeBase: EnumItem<"RigLabel">,
  read RightToeBase: EnumItem<"RigLabel">,
  read Thumb1: EnumItem<"RigLabel">,
  read Thumb2: EnumItem<"RigLabel">,
  read Thumb3: EnumItem<"RigLabel">,
  read Index1: EnumItem<"RigLabel">,
  read Index2: EnumItem<"RigLabel">,
  read Index3: EnumItem<"RigLabel">,
  read Middle1: EnumItem<"RigLabel">,
  read Middle2: EnumItem<"RigLabel">,
  read Middle3: EnumItem<"RigLabel">,
  read Ring1: EnumItem<"RigLabel">,
  read Ring2: EnumItem<"RigLabel">,
  read Ring3: EnumItem<"RigLabel">,
  read Pinky1: EnumItem<"RigLabel">,
  read Pinky2: EnumItem<"RigLabel">,
  read Pinky3: EnumItem<"RigLabel">,
}
type RigScale = {
  read Default: EnumItem<"RigScale">,
  read Rthro: EnumItem<"RigScale">,
  read RthroNarrow: EnumItem<"RigScale">,
}
type RigType = {
  read R15: EnumItem<"RigType">,
  read CustomHumanoid: EnumItem<"RigType">,
  read Custom: EnumItem<"RigType">,
  read None: EnumItem<"RigType">,
}
type RollOffMode = {
  read Inverse: EnumItem<"RollOffMode">,
  read Linear: EnumItem<"RollOffMode">,
  read LinearSquare: EnumItem<"RollOffMode">,
  read InverseTapered: EnumItem<"RollOffMode">,
}
type RolloutState = {
  read Default: EnumItem<"RolloutState">,
  read Disabled: EnumItem<"RolloutState">,
  read Enabled: EnumItem<"RolloutState">,
}
type RotationOrder = {
  read XYZ: EnumItem<"RotationOrder">,
  read XZY: EnumItem<"RotationOrder">,
  read YZX: EnumItem<"RotationOrder">,
  read YXZ: EnumItem<"RotationOrder">,
  read ZXY: EnumItem<"RotationOrder">,
  read ZYX: EnumItem<"RotationOrder">,
}
type RotationType = {
  read MovementRelative: EnumItem<"RotationType">,
  read CameraRelative: EnumItem<"RotationType">,
}
type RsvpStatus = {
  read None: EnumItem<"RsvpStatus">,
  read Going: EnumItem<"RsvpStatus">,
  read NotGoing: EnumItem<"RsvpStatus">,
}
type RtlTextSupport = {
  read Default: EnumItem<"RtlTextSupport">,
  read Disabled: EnumItem<"RtlTextSupport">,
  read Enabled: EnumItem<"RtlTextSupport">,
}
type RunContext = {
  read Legacy: EnumItem<"RunContext">,
  read Server: EnumItem<"RunContext">,
  read Client: EnumItem<"RunContext">,
  read Plugin: EnumItem<"RunContext">,
}
type RunState = {
  read Stopped: EnumItem<"RunState">,
  read Running: EnumItem<"RunState">,
  read Paused: EnumItem<"RunState">,
}
type RuntimeUndoBehavior = {
  read Aggregate: EnumItem<"RuntimeUndoBehavior">,
  read Snapshot: EnumItem<"RuntimeUndoBehavior">,
  read Hybrid: EnumItem<"RuntimeUndoBehavior">,
}
type SafeAreaCompatibility = {
  read None: EnumItem<"SafeAreaCompatibility">,
  read FullscreenExtension: EnumItem<"SafeAreaCompatibility">,
}
type SalesTypeFilter = {
  read All: EnumItem<"SalesTypeFilter">,
  read Collectibles: EnumItem<"SalesTypeFilter">,
  read Premium: EnumItem<"SalesTypeFilter">,
  read TimedOptions: EnumItem<"SalesTypeFilter">,
}
type SandboxedInstanceMode = {
  read Default: EnumItem<"SandboxedInstanceMode">,
  read Experimental: EnumItem<"SandboxedInstanceMode">,
}
type SaveAvatarThumbnailCustomizationFailure = {
  read BadThumbnailType: EnumItem<"SaveAvatarThumbnailCustomizationFailure">,
  read BadYRotDeg: EnumItem<"SaveAvatarThumbnailCustomizationFailure">,
  read BadFieldOfViewDeg: EnumItem<"SaveAvatarThumbnailCustomizationFailure">,
  read BadDistanceScale: EnumItem<"SaveAvatarThumbnailCustomizationFailure">,
  read Other: EnumItem<"SaveAvatarThumbnailCustomizationFailure">,
  read Throttled: EnumItem<"SaveAvatarThumbnailCustomizationFailure">,
}
type SaveFilter = {
  read SaveWorld: EnumItem<"SaveFilter">,
  read SaveGame: EnumItem<"SaveFilter">,
  read SaveAll: EnumItem<"SaveFilter">,
}
type SavedQualitySetting = {
  read Automatic: EnumItem<"SavedQualitySetting">,
  read QualityLevel1: EnumItem<"SavedQualitySetting">,
  read QualityLevel2: EnumItem<"SavedQualitySetting">,
  read QualityLevel3: EnumItem<"SavedQualitySetting">,
  read QualityLevel4: EnumItem<"SavedQualitySetting">,
  read QualityLevel5: EnumItem<"SavedQualitySetting">,
  read QualityLevel6: EnumItem<"SavedQualitySetting">,
  read QualityLevel7: EnumItem<"SavedQualitySetting">,
  read QualityLevel8: EnumItem<"SavedQualitySetting">,
  read QualityLevel9: EnumItem<"SavedQualitySetting">,
  read QualityLevel10: EnumItem<"SavedQualitySetting">,
}
type ScaleType = {
  read Stretch: EnumItem<"ScaleType">,
  read Slice: EnumItem<"ScaleType">,
  read Tile: EnumItem<"ScaleType">,
  read Fit: EnumItem<"ScaleType">,
  read Crop: EnumItem<"ScaleType">,
}
type ScopeCheckResult = {
  read ConsentAccepted: EnumItem<"ScopeCheckResult">,
  read InvalidScopes: EnumItem<"ScopeCheckResult">,
  read Timeout: EnumItem<"ScopeCheckResult">,
  read NoUserInput: EnumItem<"ScopeCheckResult">,
  read BackendError: EnumItem<"ScopeCheckResult">,
  read UnexpectedError: EnumItem<"ScopeCheckResult">,
  read InvalidArgument: EnumItem<"ScopeCheckResult">,
  read ConsentDenied: EnumItem<"ScopeCheckResult">,
}
type ScreenInsets = {
  read None: EnumItem<"ScreenInsets">,
  read DeviceSafeInsets: EnumItem<"ScreenInsets">,
  read CoreUISafeInsets: EnumItem<"ScreenInsets">,
  read TopbarSafeInsets: EnumItem<"ScreenInsets">,
}
type ScreenOrientation = {
  read LandscapeLeft: EnumItem<"ScreenOrientation">,
  read LandscapeRight: EnumItem<"ScreenOrientation">,
  read LandscapeSensor: EnumItem<"ScreenOrientation">,
  read Portrait: EnumItem<"ScreenOrientation">,
  read Sensor: EnumItem<"ScreenOrientation">,
}
type ScreenshotCaptureResult = {
  read Success: EnumItem<"ScreenshotCaptureResult">,
  read OtherError: EnumItem<"ScreenshotCaptureResult">,
  read NoDeviceSupport: EnumItem<"ScreenshotCaptureResult">,
  read NoSpaceOnDevice: EnumItem<"ScreenshotCaptureResult">,
}
type ScriptStoppedReason = {
  read Breakpoint: EnumItem<"ScriptStoppedReason">,
  read Exception: EnumItem<"ScriptStoppedReason">,
  read Pause: EnumItem<"ScriptStoppedReason">,
  read Step: EnumItem<"ScriptStoppedReason">,
  read Entry: EnumItem<"ScriptStoppedReason">,
}
type ScriptVariableScope = {
  read Local: EnumItem<"ScriptVariableScope">,
  read Upvalue: EnumItem<"ScriptVariableScope">,
  read Global: EnumItem<"ScriptVariableScope">,
}
type ScrollBarInset = {
  read None: EnumItem<"ScrollBarInset">,
  read ScrollBar: EnumItem<"ScrollBarInset">,
  read Always: EnumItem<"ScrollBarInset">,
}
type ScrollingDirection = {
  read X: EnumItem<"ScrollingDirection">,
  read Y: EnumItem<"ScrollingDirection">,
  read XY: EnumItem<"ScrollingDirection">,
}
type SecurityCapability = {
  read RunClientScript: EnumItem<"SecurityCapability">,
  read RunServerScript: EnumItem<"SecurityCapability">,
  read AccessOutsideWrite: EnumItem<"SecurityCapability">,
  read AssetRequire: EnumItem<"SecurityCapability">,
  read LoadString: EnumItem<"SecurityCapability">,
  read ScriptGlobals: EnumItem<"SecurityCapability">,
  read CreateInstances: EnumItem<"SecurityCapability">,
  read Basic: EnumItem<"SecurityCapability">,
  read Audio: EnumItem<"SecurityCapability">,
  read DataStore: EnumItem<"SecurityCapability">,
  read Network: EnumItem<"SecurityCapability">,
  read Physics: EnumItem<"SecurityCapability">,
  read UI: EnumItem<"SecurityCapability">,
  read CSG: EnumItem<"SecurityCapability">,
  read Chat: EnumItem<"SecurityCapability">,
  read Animation: EnumItem<"SecurityCapability">,
  read Avatar: EnumItem<"SecurityCapability">,
  read Input: EnumItem<"SecurityCapability">,
  read Environment: EnumItem<"SecurityCapability">,
  read RemoteEvent: EnumItem<"SecurityCapability">,
  read LegacySound: EnumItem<"SecurityCapability">,
  read Players: EnumItem<"SecurityCapability">,
  read CapabilityControl: EnumItem<"SecurityCapability">,
  read Plugin: EnumItem<"SecurityCapability">,
  read LocalUser: EnumItem<"SecurityCapability">,
  read WritePlayer: EnumItem<"SecurityCapability">,
  read RobloxScript: EnumItem<"SecurityCapability">,
  read RobloxEngine: EnumItem<"SecurityCapability">,
  read Unassigned: EnumItem<"SecurityCapability">,
  read InternalTest: EnumItem<"SecurityCapability">,
  read PluginOrOpenCloud: EnumItem<"SecurityCapability">,
  read Assistant: EnumItem<"SecurityCapability">,
  read RemoteCommand: EnumItem<"SecurityCapability">,
  read AssetRead: EnumItem<"SecurityCapability">,
  read AssetManagement: EnumItem<"SecurityCapability">,
  read DynamicGeneration: EnumItem<"SecurityCapability">,
  read PlatformAvatarEditing: EnumItem<"SecurityCapability">,
  read AssetCreateUpdate: EnumItem<"SecurityCapability">,
  read Capture: EnumItem<"SecurityCapability">,
  read SensitiveInput: EnumItem<"SecurityCapability">,
  read Monetization: EnumItem<"SecurityCapability">,
  read LoadOwnedAsset: EnumItem<"SecurityCapability">,
  read Social: EnumItem<"SecurityCapability">,
  read ServerCommunication: EnumItem<"SecurityCapability">,
  read Logging: EnumItem<"SecurityCapability">,
  read PromptExternalPurchase: EnumItem<"SecurityCapability">,
  read Groups: EnumItem<"SecurityCapability">,
  read Teleport: EnumItem<"SecurityCapability">,
  read Consequences: EnumItem<"SecurityCapability">,
  read Material: EnumItem<"SecurityCapability">,
  read AvatarBehavior: EnumItem<"SecurityCapability">,
  read AvatarAppearance: EnumItem<"SecurityCapability">,
  read LoadUnownedAsset: EnumItem<"SecurityCapability">,
}
type SelectionBehavior = {
  read Escape: EnumItem<"SelectionBehavior">,
  read Stop: EnumItem<"SelectionBehavior">,
}
type SelectionRenderMode = {
  read Outlines: EnumItem<"SelectionRenderMode">,
  read BoundingBoxes: EnumItem<"SelectionRenderMode">,
  read Both: EnumItem<"SelectionRenderMode">,
}
type SelfViewPosition = {
  read LastPosition: EnumItem<"SelfViewPosition">,
  read TopLeft: EnumItem<"SelfViewPosition">,
  read TopRight: EnumItem<"SelfViewPosition">,
  read BottomLeft: EnumItem<"SelfViewPosition">,
  read BottomRight: EnumItem<"SelfViewPosition">,
}
type SensorMode = {
  read Floor: EnumItem<"SensorMode">,
  read Ladder: EnumItem<"SensorMode">,
  read ClassicFloor: EnumItem<"SensorMode">,
  read ClassicLadder: EnumItem<"SensorMode">,
}
type SensorUpdateType = {
  read OnRead: EnumItem<"SensorUpdateType">,
  read Manual: EnumItem<"SensorUpdateType">,
}
type ServerLiveEditingMode = {
  read Uninitialized: EnumItem<"ServerLiveEditingMode">,
  read Enabled: EnumItem<"ServerLiveEditingMode">,
  read Disabled: EnumItem<"ServerLiveEditingMode">,
}
type ServiceVisibility = {
  read Always: EnumItem<"ServiceVisibility">,
  read Off: EnumItem<"ServiceVisibility">,
  read WithChildren: EnumItem<"ServiceVisibility">,
}
type Severity = {
  read Error: EnumItem<"Severity">,
  read Warning: EnumItem<"Severity">,
  read Information: EnumItem<"Severity">,
  read Hint: EnumItem<"Severity">,
}
type ShowAdResult = {
  read ShowCompleted: EnumItem<"ShowAdResult">,
  read AdNotReady: EnumItem<"ShowAdResult">,
  read AdAlreadyShowing: EnumItem<"ShowAdResult">,
  read InternalError: EnumItem<"ShowAdResult">,
  read ShowInterrupted: EnumItem<"ShowAdResult">,
  read InsufficientMemory: EnumItem<"ShowAdResult">,
}
type SignalBehavior = {
  read Default: EnumItem<"SignalBehavior">,
  read Immediate: EnumItem<"SignalBehavior">,
  read Deferred: EnumItem<"SignalBehavior">,
  read AncestryDeferred: EnumItem<"SignalBehavior">,
}
type SimulationMode = {
  read Default: EnumItem<"SimulationMode">,
  read Enabled: EnumItem<"SimulationMode">,
  read Disabled: EnumItem<"SimulationMode">,
}
type SizeConstraint = {
  read RelativeXY: EnumItem<"SizeConstraint">,
  read RelativeXX: EnumItem<"SizeConstraint">,
  read RelativeYY: EnumItem<"SizeConstraint">,
}
type SlimTintMode = {
  read None: EnumItem<"SlimTintMode">,
  read LOD: EnumItem<"SlimTintMode">,
  read Meshes: EnumItem<"SlimTintMode">,
  read Zone: EnumItem<"SlimTintMode">,
}
type SolidPrimitiveType = {
  read Capsule: EnumItem<"SolidPrimitiveType">,
  read Cone: EnumItem<"SolidPrimitiveType">,
  read RoundedBox: EnumItem<"SolidPrimitiveType">,
}
type SolverConvergenceMetricType = {
  read IterationBased: EnumItem<"SolverConvergenceMetricType">,
  read AlgorithmAgnostic: EnumItem<"SolverConvergenceMetricType">,
}
type SolverConvergenceVisualizationMode = {
  read Disabled: EnumItem<"SolverConvergenceVisualizationMode">,
  read PerIsland: EnumItem<"SolverConvergenceVisualizationMode">,
  read PerEdge: EnumItem<"SolverConvergenceVisualizationMode">,
}
type SortDirection = {
  read Ascending: EnumItem<"SortDirection">,
  read Descending: EnumItem<"SortDirection">,
}
type SortOrder = {
  read Name: EnumItem<"SortOrder">,
  read Custom: EnumItem<"SortOrder">,
  read LayoutOrder: EnumItem<"SortOrder">,
}
type SpecialKey = {
  read Insert: EnumItem<"SpecialKey">,
  read Home: EnumItem<"SpecialKey">,
  read End: EnumItem<"SpecialKey">,
  read PageUp: EnumItem<"SpecialKey">,
  read PageDown: EnumItem<"SpecialKey">,
  read ChatHotkey: EnumItem<"SpecialKey">,
}
type StartCorner = {
  read TopLeft: EnumItem<"StartCorner">,
  read TopRight: EnumItem<"StartCorner">,
  read BottomLeft: EnumItem<"StartCorner">,
  read BottomRight: EnumItem<"StartCorner">,
}
type StateObjectFieldType = {
  read Boolean: EnumItem<"StateObjectFieldType">,
  read CFrame: EnumItem<"StateObjectFieldType">,
  read Color3: EnumItem<"StateObjectFieldType">,
  read Float: EnumItem<"StateObjectFieldType">,
  read Instance: EnumItem<"StateObjectFieldType">,
  read Random: EnumItem<"StateObjectFieldType">,
  read Vector2: EnumItem<"StateObjectFieldType">,
  read Vector3: EnumItem<"StateObjectFieldType">,
  read INVALID: EnumItem<"StateObjectFieldType">,
}
type Status = {
  read Poison: EnumItem<"Status">,
  read Confusion: EnumItem<"Status">,
}
type StepFrequency = {
  read Hz60: EnumItem<"StepFrequency">,
  read Hz30: EnumItem<"StepFrequency">,
  read Hz15: EnumItem<"StepFrequency">,
  read Hz10: EnumItem<"StepFrequency">,
  read Hz5: EnumItem<"StepFrequency">,
  read Hz1: EnumItem<"StepFrequency">,
}
type StreamOutBehavior = {
  read Default: EnumItem<"StreamOutBehavior">,
  read LowMemory: EnumItem<"StreamOutBehavior">,
  read Opportunistic: EnumItem<"StreamOutBehavior">,
}
type StreamingIntegrityMode = {
  read Default: EnumItem<"StreamingIntegrityMode">,
  read Disabled: EnumItem<"StreamingIntegrityMode">,
  read MinimumRadiusPause: EnumItem<"StreamingIntegrityMode">,
  read PauseOutsideLoadedArea: EnumItem<"StreamingIntegrityMode">,
}
type StreamingPauseMode = {
  read Default: EnumItem<"StreamingPauseMode">,
  read Disabled: EnumItem<"StreamingPauseMode">,
  read ClientPhysicsPause: EnumItem<"StreamingPauseMode">,
}
type StrokeSizingMode = {
  read FixedSize: EnumItem<"StrokeSizingMode">,
  read ScaledSize: EnumItem<"StrokeSizingMode">,
}
type StudioAction = {
  read Undo: EnumItem<"StudioAction">,
  read Redo: EnumItem<"StudioAction">,
  read SelectAll: EnumItem<"StudioAction">,
  read DeleteSelected: EnumItem<"StudioAction">,
  read DuplicateSelection: EnumItem<"StudioAction">,
  read ClearSelection: EnumItem<"StudioAction">,
  read ZoomExtents: EnumItem<"StudioAction">,
  read Copy: EnumItem<"StudioAction">,
  read Cut: EnumItem<"StudioAction">,
  read Paste: EnumItem<"StudioAction">,
}
type StudioCaptureBufferStatus = {
  read NotStarted: EnumItem<"StudioCaptureBufferStatus">,
  read Pending: EnumItem<"StudioCaptureBufferStatus">,
  read Ready: EnumItem<"StudioCaptureBufferStatus">,
  read Error: EnumItem<"StudioCaptureBufferStatus">,
}
type StudioCaptureScreenshotFormat = {
  read RGBA8: EnumItem<"StudioCaptureScreenshotFormat">,
  read PNG: EnumItem<"StudioCaptureScreenshotFormat">,
}
type StudioCloseMode = {
  read None: EnumItem<"StudioCloseMode">,
  read CloseStudio: EnumItem<"StudioCloseMode">,
  read CloseDoc: EnumItem<"StudioCloseMode">,
  read LogOut: EnumItem<"StudioCloseMode">,
}
type StudioDataModelType = {
  read Edit: EnumItem<"StudioDataModelType">,
  read PlayClient: EnumItem<"StudioDataModelType">,
  read PlayServer: EnumItem<"StudioDataModelType">,
  read Standalone: EnumItem<"StudioDataModelType">,
  read None: EnumItem<"StudioDataModelType">,
}
type StudioPlaceUpdateFailureReason = {
  read Other: EnumItem<"StudioPlaceUpdateFailureReason">,
  read TeamCreateConflict: EnumItem<"StudioPlaceUpdateFailureReason">,
}
type StudioScriptEditorColorCategories = {
  read Default: EnumItem<"StudioScriptEditorColorCategories">,
  read Operator: EnumItem<"StudioScriptEditorColorCategories">,
  read Number: EnumItem<"StudioScriptEditorColorCategories">,
  read String: EnumItem<"StudioScriptEditorColorCategories">,
  read Comment: EnumItem<"StudioScriptEditorColorCategories">,
  read Keyword: EnumItem<"StudioScriptEditorColorCategories">,
  read Builtin: EnumItem<"StudioScriptEditorColorCategories">,
  read Method: EnumItem<"StudioScriptEditorColorCategories">,
  read Property: EnumItem<"StudioScriptEditorColorCategories">,
  read Nil: EnumItem<"StudioScriptEditorColorCategories">,
  read Bool: EnumItem<"StudioScriptEditorColorCategories">,
  read Function: EnumItem<"StudioScriptEditorColorCategories">,
  read Local: EnumItem<"StudioScriptEditorColorCategories">,
  read Self: EnumItem<"StudioScriptEditorColorCategories">,
  read LuauKeyword: EnumItem<"StudioScriptEditorColorCategories">,
  read FunctionName: EnumItem<"StudioScriptEditorColorCategories">,
  read TODO: EnumItem<"StudioScriptEditorColorCategories">,
  read Background: EnumItem<"StudioScriptEditorColorCategories">,
  read SelectionText: EnumItem<"StudioScriptEditorColorCategories">,
  read SelectionBackground: EnumItem<"StudioScriptEditorColorCategories">,
  read FindSelectionBackground: EnumItem<"StudioScriptEditorColorCategories">,
  read MatchingWordBackground: EnumItem<"StudioScriptEditorColorCategories">,
  read Warning: EnumItem<"StudioScriptEditorColorCategories">,
  read Error: EnumItem<"StudioScriptEditorColorCategories">,
  read Info: EnumItem<"StudioScriptEditorColorCategories">,
  read Hint: EnumItem<"StudioScriptEditorColorCategories">,
  read Whitespace: EnumItem<"StudioScriptEditorColorCategories">,
  read ActiveLine: EnumItem<"StudioScriptEditorColorCategories">,
  read DebuggerCurrentLine: EnumItem<"StudioScriptEditorColorCategories">,
  read DebuggerErrorLine: EnumItem<"StudioScriptEditorColorCategories">,
  read Ruler: EnumItem<"StudioScriptEditorColorCategories">,
  read Bracket: EnumItem<"StudioScriptEditorColorCategories">,
  read Type: EnumItem<"StudioScriptEditorColorCategories">,
  read MenuPrimaryText: EnumItem<"StudioScriptEditorColorCategories">,
  read MenuSecondaryText: EnumItem<"StudioScriptEditorColorCategories">,
  read MenuSelectedText: EnumItem<"StudioScriptEditorColorCategories">,
  read MenuBackground: EnumItem<"StudioScriptEditorColorCategories">,
  read MenuSelectedBackground: EnumItem<"StudioScriptEditorColorCategories">,
  read MenuScrollbarBackground: EnumItem<"StudioScriptEditorColorCategories">,
  read MenuScrollbarHandle: EnumItem<"StudioScriptEditorColorCategories">,
  read MenuBorder: EnumItem<"StudioScriptEditorColorCategories">,
  read DocViewCodeBackground: EnumItem<"StudioScriptEditorColorCategories">,
  read AICOOverlayText: EnumItem<"StudioScriptEditorColorCategories">,
  read AICOOverlayButtonBackground: EnumItem<"StudioScriptEditorColorCategories">,
  read AICOOverlayButtonBackgroundHover: EnumItem<"StudioScriptEditorColorCategories">,
  read AICOOverlayButtonBackgroundPressed: EnumItem<"StudioScriptEditorColorCategories">,
  read IndentationRuler: EnumItem<"StudioScriptEditorColorCategories">,
}
type StudioScriptEditorColorPresets = {
  read RobloxDefault: EnumItem<"StudioScriptEditorColorPresets">,
  read Extra1: EnumItem<"StudioScriptEditorColorPresets">,
  read Extra2: EnumItem<"StudioScriptEditorColorPresets">,
  read Custom: EnumItem<"StudioScriptEditorColorPresets">,
}
type StudioStyleGuideColor = {
  read MainBackground: EnumItem<"StudioStyleGuideColor">,
  read Titlebar: EnumItem<"StudioStyleGuideColor">,
  read Dropdown: EnumItem<"StudioStyleGuideColor">,
  read Tooltip: EnumItem<"StudioStyleGuideColor">,
  read Notification: EnumItem<"StudioStyleGuideColor">,
  read ScrollBar: EnumItem<"StudioStyleGuideColor">,
  read ScrollBarBackground: EnumItem<"StudioStyleGuideColor">,
  read TabBar: EnumItem<"StudioStyleGuideColor">,
  read Tab: EnumItem<"StudioStyleGuideColor">,
  read FilterButtonDefault: EnumItem<"StudioStyleGuideColor">,
  read FilterButtonHover: EnumItem<"StudioStyleGuideColor">,
  read FilterButtonChecked: EnumItem<"StudioStyleGuideColor">,
  read FilterButtonAccent: EnumItem<"StudioStyleGuideColor">,
  read FilterButtonBorder: EnumItem<"StudioStyleGuideColor">,
  read FilterButtonBorderAlt: EnumItem<"StudioStyleGuideColor">,
  read RibbonTab: EnumItem<"StudioStyleGuideColor">,
  read RibbonTabTopBar: EnumItem<"StudioStyleGuideColor">,
  read Button: EnumItem<"StudioStyleGuideColor">,
  read MainButton: EnumItem<"StudioStyleGuideColor">,
  read RibbonButton: EnumItem<"StudioStyleGuideColor">,
  read ViewPortBackground: EnumItem<"StudioStyleGuideColor">,
  read InputFieldBackground: EnumItem<"StudioStyleGuideColor">,
  read Item: EnumItem<"StudioStyleGuideColor">,
  read TableItem: EnumItem<"StudioStyleGuideColor">,
  read CategoryItem: EnumItem<"StudioStyleGuideColor">,
  read GameSettingsTableItem: EnumItem<"StudioStyleGuideColor">,
  read GameSettingsTooltip: EnumItem<"StudioStyleGuideColor">,
  read EmulatorBar: EnumItem<"StudioStyleGuideColor">,
  read EmulatorDropDown: EnumItem<"StudioStyleGuideColor">,
  read ColorPickerFrame: EnumItem<"StudioStyleGuideColor">,
  read CurrentMarker: EnumItem<"StudioStyleGuideColor">,
  read Border: EnumItem<"StudioStyleGuideColor">,
  read DropShadow: EnumItem<"StudioStyleGuideColor">,
  read Shadow: EnumItem<"StudioStyleGuideColor">,
  read Light: EnumItem<"StudioStyleGuideColor">,
  read Dark: EnumItem<"StudioStyleGuideColor">,
  read Mid: EnumItem<"StudioStyleGuideColor">,
  read MainText: EnumItem<"StudioStyleGuideColor">,
  read SubText: EnumItem<"StudioStyleGuideColor">,
  read TitlebarText: EnumItem<"StudioStyleGuideColor">,
  read BrightText: EnumItem<"StudioStyleGuideColor">,
  read DimmedText: EnumItem<"StudioStyleGuideColor">,
  read LinkText: EnumItem<"StudioStyleGuideColor">,
  read WarningText: EnumItem<"StudioStyleGuideColor">,
  read ErrorText: EnumItem<"StudioStyleGuideColor">,
  read InfoText: EnumItem<"StudioStyleGuideColor">,
  read SensitiveText: EnumItem<"StudioStyleGuideColor">,
  read ScriptSideWidget: EnumItem<"StudioStyleGuideColor">,
  read ScriptBackground: EnumItem<"StudioStyleGuideColor">,
  read ScriptText: EnumItem<"StudioStyleGuideColor">,
  read ScriptSelectionText: EnumItem<"StudioStyleGuideColor">,
  read ScriptSelectionBackground: EnumItem<"StudioStyleGuideColor">,
  read ScriptFindSelectionBackground: EnumItem<"StudioStyleGuideColor">,
  read ScriptMatchingWordSelectionBackground: EnumItem<"StudioStyleGuideColor">,
  read ScriptOperator: EnumItem<"StudioStyleGuideColor">,
  read ScriptNumber: EnumItem<"StudioStyleGuideColor">,
  read ScriptString: EnumItem<"StudioStyleGuideColor">,
  read ScriptComment: EnumItem<"StudioStyleGuideColor">,
  read ScriptKeyword: EnumItem<"StudioStyleGuideColor">,
  read ScriptBuiltInFunction: EnumItem<"StudioStyleGuideColor">,
  read ScriptWarning: EnumItem<"StudioStyleGuideColor">,
  read ScriptError: EnumItem<"StudioStyleGuideColor">,
  read ScriptInformation: EnumItem<"StudioStyleGuideColor">,
  read ScriptHint: EnumItem<"StudioStyleGuideColor">,
  read ScriptWhitespace: EnumItem<"StudioStyleGuideColor">,
  read ScriptRuler: EnumItem<"StudioStyleGuideColor">,
  read DocViewCodeBackground: EnumItem<"StudioStyleGuideColor">,
  read DebuggerCurrentLine: EnumItem<"StudioStyleGuideColor">,
  read DebuggerErrorLine: EnumItem<"StudioStyleGuideColor">,
  read DiffFilePathText: EnumItem<"StudioStyleGuideColor">,
  read DiffTextHunkInfo: EnumItem<"StudioStyleGuideColor">,
  read DiffTextNoChange: EnumItem<"StudioStyleGuideColor">,
  read DiffTextAddition: EnumItem<"StudioStyleGuideColor">,
  read DiffTextDeletion: EnumItem<"StudioStyleGuideColor">,
  read DiffTextSeparatorBackground: EnumItem<"StudioStyleGuideColor">,
  read DiffTextNoChangeBackground: EnumItem<"StudioStyleGuideColor">,
  read DiffTextAdditionBackground: EnumItem<"StudioStyleGuideColor">,
  read DiffTextDeletionBackground: EnumItem<"StudioStyleGuideColor">,
  read DiffLineNum: EnumItem<"StudioStyleGuideColor">,
  read DiffLineNumSeparatorBackground: EnumItem<"StudioStyleGuideColor">,
  read DiffLineNumNoChangeBackground: EnumItem<"StudioStyleGuideColor">,
  read DiffLineNumAdditionBackground: EnumItem<"StudioStyleGuideColor">,
  read DiffLineNumDeletionBackground: EnumItem<"StudioStyleGuideColor">,
  read DiffFilePathBackground: EnumItem<"StudioStyleGuideColor">,
  read DiffFilePathBorder: EnumItem<"StudioStyleGuideColor">,
  read ChatIncomingBgColor: EnumItem<"StudioStyleGuideColor">,
  read ChatIncomingTextColor: EnumItem<"StudioStyleGuideColor">,
  read ChatOutgoingBgColor: EnumItem<"StudioStyleGuideColor">,
  read ChatOutgoingTextColor: EnumItem<"StudioStyleGuideColor">,
  read ChatModeratedMessageColor: EnumItem<"StudioStyleGuideColor">,
  read Separator: EnumItem<"StudioStyleGuideColor">,
  read ButtonBorder: EnumItem<"StudioStyleGuideColor">,
  read ButtonText: EnumItem<"StudioStyleGuideColor">,
  read InputFieldBorder: EnumItem<"StudioStyleGuideColor">,
  read CheckedFieldBackground: EnumItem<"StudioStyleGuideColor">,
  read CheckedFieldBorder: EnumItem<"StudioStyleGuideColor">,
  read CheckedFieldIndicator: EnumItem<"StudioStyleGuideColor">,
  read HeaderSection: EnumItem<"StudioStyleGuideColor">,
  read Midlight: EnumItem<"StudioStyleGuideColor">,
  read StatusBar: EnumItem<"StudioStyleGuideColor">,
  read DialogButton: EnumItem<"StudioStyleGuideColor">,
  read DialogButtonText: EnumItem<"StudioStyleGuideColor">,
  read DialogButtonBorder: EnumItem<"StudioStyleGuideColor">,
  read DialogMainButton: EnumItem<"StudioStyleGuideColor">,
  read DialogMainButtonText: EnumItem<"StudioStyleGuideColor">,
  read InfoBarWarningBackground: EnumItem<"StudioStyleGuideColor">,
  read InfoBarWarningText: EnumItem<"StudioStyleGuideColor">,
  read ScriptEditorCurrentLine: EnumItem<"StudioStyleGuideColor">,
  read ScriptMethod: EnumItem<"StudioStyleGuideColor">,
  read ScriptProperty: EnumItem<"StudioStyleGuideColor">,
  read ScriptNil: EnumItem<"StudioStyleGuideColor">,
  read ScriptBool: EnumItem<"StudioStyleGuideColor">,
  read ScriptFunction: EnumItem<"StudioStyleGuideColor">,
  read ScriptLocal: EnumItem<"StudioStyleGuideColor">,
  read ScriptSelf: EnumItem<"StudioStyleGuideColor">,
  read ScriptLuauKeyword: EnumItem<"StudioStyleGuideColor">,
  read ScriptFunctionName: EnumItem<"StudioStyleGuideColor">,
  read ScriptTodo: EnumItem<"StudioStyleGuideColor">,
  read ScriptBracket: EnumItem<"StudioStyleGuideColor">,
  read AttributeCog: EnumItem<"StudioStyleGuideColor">,
  read AICOOverlayText: EnumItem<"StudioStyleGuideColor">,
  read AICOOverlayButtonBackground: EnumItem<"StudioStyleGuideColor">,
  read AICOOverlayButtonBackgroundHover: EnumItem<"StudioStyleGuideColor">,
  read AICOOverlayButtonBackgroundPressed: EnumItem<"StudioStyleGuideColor">,
  read OnboardingCover: EnumItem<"StudioStyleGuideColor">,
  read OnboardingHighlight: EnumItem<"StudioStyleGuideColor">,
  read OnboardingShadow: EnumItem<"StudioStyleGuideColor">,
  read BreakpointMarker: EnumItem<"StudioStyleGuideColor">,
  read DiffLineNumHover: EnumItem<"StudioStyleGuideColor">,
  read DiffLineNumSeparatorBackgroundHover: EnumItem<"StudioStyleGuideColor">,
}
type StudioStyleGuideModifier = {
  read Default: EnumItem<"StudioStyleGuideModifier">,
  read Selected: EnumItem<"StudioStyleGuideModifier">,
  read Pressed: EnumItem<"StudioStyleGuideModifier">,
  read Disabled: EnumItem<"StudioStyleGuideModifier">,
  read Hover: EnumItem<"StudioStyleGuideModifier">,
}
type Style = {
  read AlternatingSupports: EnumItem<"Style">,
  read BridgeStyleSupports: EnumItem<"Style">,
  read NoSupports: EnumItem<"Style">,
}
type SubscriptionExpirationReason = {
  read ProductInactive: EnumItem<"SubscriptionExpirationReason">,
  read ProductDeleted: EnumItem<"SubscriptionExpirationReason">,
  read SubscriberCancelled: EnumItem<"SubscriptionExpirationReason">,
  read SubscriberRefunded: EnumItem<"SubscriptionExpirationReason">,
  read Lapsed: EnumItem<"SubscriptionExpirationReason">,
}
type SubscriptionPaymentStatus = {
  read Paid: EnumItem<"SubscriptionPaymentStatus">,
  read Refunded: EnumItem<"SubscriptionPaymentStatus">,
}
type SubscriptionPeriod = {
  read Month: EnumItem<"SubscriptionPeriod">,
}
type SubscriptionState = {
  read NeverSubscribed: EnumItem<"SubscriptionState">,
  read SubscribedWillRenew: EnumItem<"SubscriptionState">,
  read SubscribedWillNotRenew: EnumItem<"SubscriptionState">,
  read SubscribedRenewalPaymentPending: EnumItem<"SubscriptionState">,
  read Expired: EnumItem<"SubscriptionState">,
}
type SurfaceConstraint = {
  read None: EnumItem<"SurfaceConstraint">,
  read Hinge: EnumItem<"SurfaceConstraint">,
  read SteppingMotor: EnumItem<"SurfaceConstraint">,
  read Motor: EnumItem<"SurfaceConstraint">,
}
type SurfaceGuiShape = {
  read Flat: EnumItem<"SurfaceGuiShape">,
  read CurvedHorizontally: EnumItem<"SurfaceGuiShape">,
}
type SurfaceGuiSizingMode = {
  read FixedSize: EnumItem<"SurfaceGuiSizingMode">,
  read PixelsPerStud: EnumItem<"SurfaceGuiSizingMode">,
}
type SurfaceType = {
  read Smooth: EnumItem<"SurfaceType">,
  read Glue: EnumItem<"SurfaceType">,
  read Weld: EnumItem<"SurfaceType">,
  read Studs: EnumItem<"SurfaceType">,
  read Inlet: EnumItem<"SurfaceType">,
  read Universal: EnumItem<"SurfaceType">,
  read Hinge: EnumItem<"SurfaceType">,
  read Motor: EnumItem<"SurfaceType">,
  read SteppingMotor: EnumItem<"SurfaceType">,
  read SmoothNoOutlines: EnumItem<"SurfaceType">,
}
type SwipeDirection = {
  read Right: EnumItem<"SwipeDirection">,
  read Left: EnumItem<"SwipeDirection">,
  read Up: EnumItem<"SwipeDirection">,
  read Down: EnumItem<"SwipeDirection">,
  read None: EnumItem<"SwipeDirection">,
}
type SystemThemeValue = {
  read error: EnumItem<"SystemThemeValue">,
  read light: EnumItem<"SystemThemeValue">,
  read dark: EnumItem<"SystemThemeValue">,
  read systemLight: EnumItem<"SystemThemeValue">,
  read systemDark: EnumItem<"SystemThemeValue">,
}
type TableMajorAxis = {
  read RowMajor: EnumItem<"TableMajorAxis">,
  read ColumnMajor: EnumItem<"TableMajorAxis">,
}
type TagReplicability = {}
type TeamCreateErrorState = {
  read PlaceSizeTooLarge: EnumItem<"TeamCreateErrorState">,
  read PlaceSizeApproachingLimit: EnumItem<"TeamCreateErrorState">,
  read PlaceUploadFailing: EnumItem<"TeamCreateErrorState">,
  read NoError: EnumItem<"TeamCreateErrorState">,
}
type Technology = {
  read Legacy: EnumItem<"Technology">,
  read Voxel: EnumItem<"Technology">,
  read Compatibility: EnumItem<"Technology">,
  read ShadowMap: EnumItem<"Technology">,
  read Future: EnumItem<"Technology">,
  read Unified: EnumItem<"Technology">,
}
type TelemetryBackend = {
  read UNSPECIFIED: EnumItem<"TelemetryBackend">,
  read EventIngest: EnumItem<"TelemetryBackend">,
  read Points: EnumItem<"TelemetryBackend">,
  read Teletune: EnumItem<"TelemetryBackend">,
  read EphemeralCounter: EnumItem<"TelemetryBackend">,
  read EphemeralStat: EnumItem<"TelemetryBackend">,
  read Counter: EnumItem<"TelemetryBackend">,
  read Stat: EnumItem<"TelemetryBackend">,
}
type TelemetryStandardizedField = {
  read AddDatacenterId: EnumItem<"TelemetryStandardizedField">,
  read AddPlaceId: EnumItem<"TelemetryStandardizedField">,
  read AddUniverseId: EnumItem<"TelemetryStandardizedField">,
  read AddPlaceInstanceId: EnumItem<"TelemetryStandardizedField">,
  read AddPlaySessionId: EnumItem<"TelemetryStandardizedField">,
  read AddCurrentContextName: EnumItem<"TelemetryStandardizedField">,
  read AddOsInfo: EnumItem<"TelemetryStandardizedField">,
  read AddArchitectureInfo: EnumItem<"TelemetryStandardizedField">,
  read AddCpuInfo: EnumItem<"TelemetryStandardizedField">,
  read AddMemoryInfo: EnumItem<"TelemetryStandardizedField">,
  read AddSessionInfo: EnumItem<"TelemetryStandardizedField">,
}
type TeleportMethod = {
  read TeleportToSpawnByName: EnumItem<"TeleportMethod">,
  read TeleportToPlaceInstance: EnumItem<"TeleportMethod">,
  read TeleportToPrivateServer: EnumItem<"TeleportMethod">,
  read TeleportPartyAsync: EnumItem<"TeleportMethod">,
  read TeleportToVIPServer: EnumItem<"TeleportMethod">,
  read TeleportToInstanceBack: EnumItem<"TeleportMethod">,
  read TeleportUnknown: EnumItem<"TeleportMethod">,
}
type TeleportResult = {
  read Success: EnumItem<"TeleportResult">,
  read Failure: EnumItem<"TeleportResult">,
  read GameNotFound: EnumItem<"TeleportResult">,
  read GameEnded: EnumItem<"TeleportResult">,
  read GameFull: EnumItem<"TeleportResult">,
  read Unauthorized: EnumItem<"TeleportResult">,
  read Flooded: EnumItem<"TeleportResult">,
  read IsTeleporting: EnumItem<"TeleportResult">,
}
type TeleportState = {
  read RequestedFromServer: EnumItem<"TeleportState">,
  read Started: EnumItem<"TeleportState">,
  read WaitingForServer: EnumItem<"TeleportState">,
  read Failed: EnumItem<"TeleportState">,
  read InProgress: EnumItem<"TeleportState">,
}
type TeleportType = {
  read ToPlace: EnumItem<"TeleportType">,
  read ToInstance: EnumItem<"TeleportType">,
  read ToReservedServer: EnumItem<"TeleportType">,
  read ToVIPServer: EnumItem<"TeleportType">,
  read ToInstanceBack: EnumItem<"TeleportType">,
}
type TerrainAcquisitionMethod = {
  read None: EnumItem<"TerrainAcquisitionMethod">,
  read Legacy: EnumItem<"TerrainAcquisitionMethod">,
  read Template: EnumItem<"TerrainAcquisitionMethod">,
  read Generate: EnumItem<"TerrainAcquisitionMethod">,
  read Import: EnumItem<"TerrainAcquisitionMethod">,
  read Convert: EnumItem<"TerrainAcquisitionMethod">,
  read EditAddTool: EnumItem<"TerrainAcquisitionMethod">,
  read EditSeaLevelTool: EnumItem<"TerrainAcquisitionMethod">,
  read EditReplaceTool: EnumItem<"TerrainAcquisitionMethod">,
  read RegionFillTool: EnumItem<"TerrainAcquisitionMethod">,
  read RegionPasteTool: EnumItem<"TerrainAcquisitionMethod">,
  read Other: EnumItem<"TerrainAcquisitionMethod">,
}
type TerrainFace = {
  read Top: EnumItem<"TerrainFace">,
  read Side: EnumItem<"TerrainFace">,
  read Bottom: EnumItem<"TerrainFace">,
}
type TerrainLiquidMergeOperation = {
  read None: EnumItem<"TerrainLiquidMergeOperation">,
  read Source: EnumItem<"TerrainLiquidMergeOperation">,
  read Union: EnumItem<"TerrainLiquidMergeOperation">,
  read Difference: EnumItem<"TerrainLiquidMergeOperation">,
  read Intersect: EnumItem<"TerrainLiquidMergeOperation">,
}
type TerrainSolidMergeOperation = {
  read None: EnumItem<"TerrainSolidMergeOperation">,
  read Paint: EnumItem<"TerrainSolidMergeOperation">,
  read Source: EnumItem<"TerrainSolidMergeOperation">,
  read Union: EnumItem<"TerrainSolidMergeOperation">,
  read Dig: EnumItem<"TerrainSolidMergeOperation">,
  read Difference: EnumItem<"TerrainSolidMergeOperation">,
  read Intersect: EnumItem<"TerrainSolidMergeOperation">,
  read Cut: EnumItem<"TerrainSolidMergeOperation">,
  read Place: EnumItem<"TerrainSolidMergeOperation">,
}
type TextChatMessageStatus = {
  read Unknown: EnumItem<"TextChatMessageStatus">,
  read Success: EnumItem<"TextChatMessageStatus">,
  read Sending: EnumItem<"TextChatMessageStatus">,
  read TextFilterFailed: EnumItem<"TextChatMessageStatus">,
  read Floodchecked: EnumItem<"TextChatMessageStatus">,
  read InvalidPrivacySettings: EnumItem<"TextChatMessageStatus">,
  read InvalidTextChannelPermissions: EnumItem<"TextChatMessageStatus">,
  read MessageTooLong: EnumItem<"TextChatMessageStatus">,
  read ModerationTimeout: EnumItem<"TextChatMessageStatus">,
}
type TextDirection = {
  read Auto: EnumItem<"TextDirection">,
  read LeftToRight: EnumItem<"TextDirection">,
  read RightToLeft: EnumItem<"TextDirection">,
}
type TextFilterContext = {
  read PublicChat: EnumItem<"TextFilterContext">,
  read PrivateChat: EnumItem<"TextFilterContext">,
}
type TextInputType = {
  read Default: EnumItem<"TextInputType">,
  read NoSuggestions: EnumItem<"TextInputType">,
  read Number: EnumItem<"TextInputType">,
  read Email: EnumItem<"TextInputType">,
  read Phone: EnumItem<"TextInputType">,
  read Password: EnumItem<"TextInputType">,
  read PasswordShown: EnumItem<"TextInputType">,
  read Username: EnumItem<"TextInputType">,
  read OneTimePassword: EnumItem<"TextInputType">,
  read NewPassword: EnumItem<"TextInputType">,
  read NewPasswordShown: EnumItem<"TextInputType">,
}
type TextTruncate = {
  read None: EnumItem<"TextTruncate">,
  read AtEnd: EnumItem<"TextTruncate">,
  read SplitWord: EnumItem<"TextTruncate">,
}
type TextXAlignment = {
  read Left: EnumItem<"TextXAlignment">,
  read Right: EnumItem<"TextXAlignment">,
  read Center: EnumItem<"TextXAlignment">,
}
type TextYAlignment = {
  read Top: EnumItem<"TextYAlignment">,
  read Center: EnumItem<"TextYAlignment">,
  read Bottom: EnumItem<"TextYAlignment">,
}
type TextureMode = {
  read Stretch: EnumItem<"TextureMode">,
  read Wrap: EnumItem<"TextureMode">,
  read Static: EnumItem<"TextureMode">,
}
type TextureQueryType = {
  read NonHumanoid: EnumItem<"TextureQueryType">,
  read NonHumanoidOrphaned: EnumItem<"TextureQueryType">,
  read Humanoid: EnumItem<"TextureQueryType">,
  read HumanoidOrphaned: EnumItem<"TextureQueryType">,
}
type ThreadPoolConfig = {
  read Auto: EnumItem<"ThreadPoolConfig">,
  read Threads1: EnumItem<"ThreadPoolConfig">,
  read Threads2: EnumItem<"ThreadPoolConfig">,
  read Threads3: EnumItem<"ThreadPoolConfig">,
  read Threads4: EnumItem<"ThreadPoolConfig">,
  read Threads8: EnumItem<"ThreadPoolConfig">,
  read Threads16: EnumItem<"ThreadPoolConfig">,
  read PerCore1: EnumItem<"ThreadPoolConfig">,
  read PerCore2: EnumItem<"ThreadPoolConfig">,
  read PerCore3: EnumItem<"ThreadPoolConfig">,
  read PerCore4: EnumItem<"ThreadPoolConfig">,
}
type ThrottlingPriority = {
  read Default: EnumItem<"ThrottlingPriority">,
  read ElevatedOnServer: EnumItem<"ThrottlingPriority">,
  read Extreme: EnumItem<"ThrottlingPriority">,
}
type ThumbnailSize = {
  read Size48x48: EnumItem<"ThumbnailSize">,
  read Size180x180: EnumItem<"ThumbnailSize">,
  read Size420x420: EnumItem<"ThumbnailSize">,
  read Size60x60: EnumItem<"ThumbnailSize">,
  read Size100x100: EnumItem<"ThumbnailSize">,
  read Size150x150: EnumItem<"ThumbnailSize">,
  read Size352x352: EnumItem<"ThumbnailSize">,
}
type ThumbnailType = {
  read HeadShot: EnumItem<"ThumbnailType">,
  read AvatarBust: EnumItem<"ThumbnailType">,
  read AvatarThumbnail: EnumItem<"ThumbnailType">,
}
type TickCountSampleMethod = {
  read Fast: EnumItem<"TickCountSampleMethod">,
  read Benchmark: EnumItem<"TickCountSampleMethod">,
  read Precise: EnumItem<"TickCountSampleMethod">,
}
type TonemapperPreset = {
  read Default: EnumItem<"TonemapperPreset">,
  read Retro: EnumItem<"TonemapperPreset">,
}
type TopBottom = {
  read Top: EnumItem<"TopBottom">,
  read Center: EnumItem<"TopBottom">,
  read Bottom: EnumItem<"TopBottom">,
}
type TouchCameraMovementMode = {
  read Default: EnumItem<"TouchCameraMovementMode">,
  read Classic: EnumItem<"TouchCameraMovementMode">,
  read Follow: EnumItem<"TouchCameraMovementMode">,
  read Orbital: EnumItem<"TouchCameraMovementMode">,
}
type TouchMovementMode = {
  read Default: EnumItem<"TouchMovementMode">,
  read Thumbstick: EnumItem<"TouchMovementMode">,
  read DPad: EnumItem<"TouchMovementMode">,
  read Thumbpad: EnumItem<"TouchMovementMode">,
  read ClickToMove: EnumItem<"TouchMovementMode">,
  read DynamicThumbstick: EnumItem<"TouchMovementMode">,
}
type TrackerError = {
  read Ok: EnumItem<"TrackerError">,
  read NoService: EnumItem<"TrackerError">,
  read InitFailed: EnumItem<"TrackerError">,
  read NoVideo: EnumItem<"TrackerError">,
  read VideoError: EnumItem<"TrackerError">,
  read VideoNoPermission: EnumItem<"TrackerError">,
  read VideoUnsupported: EnumItem<"TrackerError">,
  read NoAudio: EnumItem<"TrackerError">,
  read AudioError: EnumItem<"TrackerError">,
  read AudioNoPermission: EnumItem<"TrackerError">,
  read UnsupportedDevice: EnumItem<"TrackerError">,
}
type TrackerExtrapolationFlagMode = {
  read ForceDisabled: EnumItem<"TrackerExtrapolationFlagMode">,
  read ExtrapolateFacsAndPose: EnumItem<"TrackerExtrapolationFlagMode">,
  read ExtrapolateFacsOnly: EnumItem<"TrackerExtrapolationFlagMode">,
  read Auto: EnumItem<"TrackerExtrapolationFlagMode">,
}
type TrackerFaceTrackingStatus = {
  read FaceTrackingSuccess: EnumItem<"TrackerFaceTrackingStatus">,
  read FaceTrackingNoFaceFound: EnumItem<"TrackerFaceTrackingStatus">,
  read FaceTrackingUnknown: EnumItem<"TrackerFaceTrackingStatus">,
  read FaceTrackingLost: EnumItem<"TrackerFaceTrackingStatus">,
  read FaceTrackingHasTrackingError: EnumItem<"TrackerFaceTrackingStatus">,
  read FaceTrackingIsOccluded: EnumItem<"TrackerFaceTrackingStatus">,
  read FaceTrackingUninitialized: EnumItem<"TrackerFaceTrackingStatus">,
}
type TrackerLodFlagMode = {
  read ForceFalse: EnumItem<"TrackerLodFlagMode">,
  read ForceTrue: EnumItem<"TrackerLodFlagMode">,
  read Auto: EnumItem<"TrackerLodFlagMode">,
}
type TrackerLodValueMode = {
  read Force0: EnumItem<"TrackerLodValueMode">,
  read Force1: EnumItem<"TrackerLodValueMode">,
  read Auto: EnumItem<"TrackerLodValueMode">,
}
type TrackerMode = {
  read None: EnumItem<"TrackerMode">,
  read Audio: EnumItem<"TrackerMode">,
  read Video: EnumItem<"TrackerMode">,
  read AudioVideo: EnumItem<"TrackerMode">,
}
type TrackerPromptEvent = {
  read LODCameraRecommendDisable: EnumItem<"TrackerPromptEvent">,
}
type TrackerType = {
  read None: EnumItem<"TrackerType">,
  read Face: EnumItem<"TrackerType">,
  read UpperBody: EnumItem<"TrackerType">,
}
type TriStateBoolean = {
  read Unknown: EnumItem<"TriStateBoolean">,
  read True: EnumItem<"TriStateBoolean">,
  read False: EnumItem<"TriStateBoolean">,
}
type TweenStatus = {
  read Canceled: EnumItem<"TweenStatus">,
  read Completed: EnumItem<"TweenStatus">,
}
type UICaptureMode = {
  read All: EnumItem<"UICaptureMode">,
  read None: EnumItem<"UICaptureMode">,
}
type UIDragDetectorBoundingBehavior = {
  read Automatic: EnumItem<"UIDragDetectorBoundingBehavior">,
  read EntireObject: EnumItem<"UIDragDetectorBoundingBehavior">,
  read HitPoint: EnumItem<"UIDragDetectorBoundingBehavior">,
}
type UIDragDetectorDragRelativity = {
  read Absolute: EnumItem<"UIDragDetectorDragRelativity">,
  read Relative: EnumItem<"UIDragDetectorDragRelativity">,
}
type UIDragDetectorDragSpace = {
  read Parent: EnumItem<"UIDragDetectorDragSpace">,
  read LayerCollector: EnumItem<"UIDragDetectorDragSpace">,
  read Reference: EnumItem<"UIDragDetectorDragSpace">,
}
type UIDragDetectorDragStyle = {
  read TranslatePlane: EnumItem<"UIDragDetectorDragStyle">,
  read TranslateLine: EnumItem<"UIDragDetectorDragStyle">,
  read Rotate: EnumItem<"UIDragDetectorDragStyle">,
  read Scriptable: EnumItem<"UIDragDetectorDragStyle">,
}
type UIDragDetectorResponseStyle = {
  read Offset: EnumItem<"UIDragDetectorResponseStyle">,
  read Scale: EnumItem<"UIDragDetectorResponseStyle">,
  read CustomOffset: EnumItem<"UIDragDetectorResponseStyle">,
  read CustomScale: EnumItem<"UIDragDetectorResponseStyle">,
}
type UIDragSpeedAxisMapping = {
  read XY: EnumItem<"UIDragSpeedAxisMapping">,
  read XX: EnumItem<"UIDragSpeedAxisMapping">,
  read YY: EnumItem<"UIDragSpeedAxisMapping">,
}
type UIFlexAlignment = {
  read None: EnumItem<"UIFlexAlignment">,
  read Fill: EnumItem<"UIFlexAlignment">,
  read SpaceAround: EnumItem<"UIFlexAlignment">,
  read SpaceBetween: EnumItem<"UIFlexAlignment">,
  read SpaceEvenly: EnumItem<"UIFlexAlignment">,
}
type UIFlexMode = {
  read None: EnumItem<"UIFlexMode">,
  read Grow: EnumItem<"UIFlexMode">,
  read Shrink: EnumItem<"UIFlexMode">,
  read Fill: EnumItem<"UIFlexMode">,
  read Custom: EnumItem<"UIFlexMode">,
}
type UITheme = {
  read Light: EnumItem<"UITheme">,
  read Dark: EnumItem<"UITheme">,
}
type UiMessageType = {
  read UiMessageError: EnumItem<"UiMessageType">,
  read UiMessageInfo: EnumItem<"UiMessageType">,
}
type UpdateState = {
  read UpdateNotAvailable: EnumItem<"UpdateState">,
  read UpdateAvailable: EnumItem<"UpdateState">,
  read UpdateInProgress: EnumItem<"UpdateState">,
  read UpdateReady: EnumItem<"UpdateState">,
  read UpdateFailed: EnumItem<"UpdateState">,
}
type UploadCaptureResult = {
  read Success: EnumItem<"UploadCaptureResult">,
  read NeedPermission: EnumItem<"UploadCaptureResult">,
  read CaptureModerated: EnumItem<"UploadCaptureResult">,
  read CaptureNotInGallery: EnumItem<"UploadCaptureResult">,
  read IneligibleCapture: EnumItem<"UploadCaptureResult">,
  read UploadQuotaReached: EnumItem<"UploadCaptureResult">,
  read UploadPending: EnumItem<"UploadCaptureResult">,
  read UploadFailed: EnumItem<"UploadCaptureResult">,
}
type UsageContext = {
  read Default: EnumItem<"UsageContext">,
  read Preview: EnumItem<"UsageContext">,
}
type UserCFrame = {
  read Head: EnumItem<"UserCFrame">,
  read LeftHand: EnumItem<"UserCFrame">,
  read RightHand: EnumItem<"UserCFrame">,
  read Floor: EnumItem<"UserCFrame">,
}
type UserInputState = {
  read Begin: EnumItem<"UserInputState">,
  read Change: EnumItem<"UserInputState">,
  read End: EnumItem<"UserInputState">,
  read Cancel: EnumItem<"UserInputState">,
  read None: EnumItem<"UserInputState">,
}
type UserInputType = {
  read MouseButton1: EnumItem<"UserInputType">,
  read MouseButton2: EnumItem<"UserInputType">,
  read MouseButton3: EnumItem<"UserInputType">,
  read MouseWheel: EnumItem<"UserInputType">,
  read MouseMovement: EnumItem<"UserInputType">,
  read Touch: EnumItem<"UserInputType">,
  read Keyboard: EnumItem<"UserInputType">,
  read Focus: EnumItem<"UserInputType">,
  read Accelerometer: EnumItem<"UserInputType">,
  read Gyro: EnumItem<"UserInputType">,
  read Gamepad1: EnumItem<"UserInputType">,
  read Gamepad2: EnumItem<"UserInputType">,
  read Gamepad3: EnumItem<"UserInputType">,
  read Gamepad4: EnumItem<"UserInputType">,
  read Gamepad5: EnumItem<"UserInputType">,
  read Gamepad6: EnumItem<"UserInputType">,
  read Gamepad7: EnumItem<"UserInputType">,
  read Gamepad8: EnumItem<"UserInputType">,
  read TextInput: EnumItem<"UserInputType">,
  read InputMethod: EnumItem<"UserInputType">,
  read None: EnumItem<"UserInputType">,
}
type VRComfortSetting = {
  read Comfort: EnumItem<"VRComfortSetting">,
  read Normal: EnumItem<"VRComfortSetting">,
  read Expert: EnumItem<"VRComfortSetting">,
  read Custom: EnumItem<"VRComfortSetting">,
}
type VRControllerModelMode = {
  read Disabled: EnumItem<"VRControllerModelMode">,
  read Transparent: EnumItem<"VRControllerModelMode">,
}
type VRDeviceType = {
  read Unknown: EnumItem<"VRDeviceType">,
  read OculusRift: EnumItem<"VRDeviceType">,
  read HTCVive: EnumItem<"VRDeviceType">,
  read ValveIndex: EnumItem<"VRDeviceType">,
  read OculusQuest: EnumItem<"VRDeviceType">,
}
type VRLaserPointerMode = {
  read Disabled: EnumItem<"VRLaserPointerMode">,
  read Pointer: EnumItem<"VRLaserPointerMode">,
  read DualPointer: EnumItem<"VRLaserPointerMode">,
}
type VRSafetyBubbleMode = {
  read NoOne: EnumItem<"VRSafetyBubbleMode">,
  read OnlyFriends: EnumItem<"VRSafetyBubbleMode">,
  read Anyone: EnumItem<"VRSafetyBubbleMode">,
}
type VRScaling = {
  read World: EnumItem<"VRScaling">,
  read Off: EnumItem<"VRScaling">,
}
type VRSessionState = {
  read Undefined: EnumItem<"VRSessionState">,
  read Idle: EnumItem<"VRSessionState">,
  read Visible: EnumItem<"VRSessionState">,
  read Focused: EnumItem<"VRSessionState">,
  read Stopping: EnumItem<"VRSessionState">,
}
type VRTouchpad = {
  read Left: EnumItem<"VRTouchpad">,
  read Right: EnumItem<"VRTouchpad">,
}
type VRTouchpadMode = {
  read Touch: EnumItem<"VRTouchpadMode">,
  read VirtualThumbstick: EnumItem<"VRTouchpadMode">,
  read ABXY: EnumItem<"VRTouchpadMode">,
}
type VelocityConstraintMode = {
  read Line: EnumItem<"VelocityConstraintMode">,
  read Plane: EnumItem<"VelocityConstraintMode">,
  read Vector: EnumItem<"VelocityConstraintMode">,
}
type VerticalAlignment = {
  read Center: EnumItem<"VerticalAlignment">,
  read Top: EnumItem<"VerticalAlignment">,
  read Bottom: EnumItem<"VerticalAlignment">,
}
type VerticalScrollBarPosition = {
  read Right: EnumItem<"VerticalScrollBarPosition">,
  read Left: EnumItem<"VerticalScrollBarPosition">,
}
type VibrationMotor = {
  read Large: EnumItem<"VibrationMotor">,
  read Small: EnumItem<"VibrationMotor">,
  read LeftTrigger: EnumItem<"VibrationMotor">,
  read RightTrigger: EnumItem<"VibrationMotor">,
  read LeftHand: EnumItem<"VibrationMotor">,
  read RightHand: EnumItem<"VibrationMotor">,
}
type VideoCaptureResult = {
  read Success: EnumItem<"VideoCaptureResult">,
  read OtherError: EnumItem<"VideoCaptureResult">,
  read ScreenSizeChanged: EnumItem<"VideoCaptureResult">,
  read TimeLimitReached: EnumItem<"VideoCaptureResult">,
}
type VideoCaptureStartedResult = {
  read Success: EnumItem<"VideoCaptureStartedResult">,
  read OtherError: EnumItem<"VideoCaptureStartedResult">,
  read CapturingAlready: EnumItem<"VideoCaptureStartedResult">,
  read NoDeviceSupport: EnumItem<"VideoCaptureStartedResult">,
  read NoSpaceOnDevice: EnumItem<"VideoCaptureStartedResult">,
}
type VideoDeviceCaptureQuality = {
  read Default: EnumItem<"VideoDeviceCaptureQuality">,
  read Low: EnumItem<"VideoDeviceCaptureQuality">,
  read Medium: EnumItem<"VideoDeviceCaptureQuality">,
  read High: EnumItem<"VideoDeviceCaptureQuality">,
}
type VideoError = {
  read Ok: EnumItem<"VideoError">,
  read Eof: EnumItem<"VideoError">,
  read EAgain: EnumItem<"VideoError">,
  read BadParameter: EnumItem<"VideoError">,
  read AllocFailed: EnumItem<"VideoError">,
  read CodecInitFailed: EnumItem<"VideoError">,
  read CodecCloseFailed: EnumItem<"VideoError">,
  read DecodeFailed: EnumItem<"VideoError">,
  read ParsingFailed: EnumItem<"VideoError">,
  read Unsupported: EnumItem<"VideoError">,
  read Generic: EnumItem<"VideoError">,
  read DownloadFailed: EnumItem<"VideoError">,
  read StreamNotFound: EnumItem<"VideoError">,
  read EncodeFailed: EnumItem<"VideoError">,
  read CreateFailed: EnumItem<"VideoError">,
  read NoPermission: EnumItem<"VideoError">,
  read NoService: EnumItem<"VideoError">,
  read ReleaseFailed: EnumItem<"VideoError">,
  read Unknown: EnumItem<"VideoError">,
}
type VideoSampleSize = {
  read Small: EnumItem<"VideoSampleSize">,
  read Medium: EnumItem<"VideoSampleSize">,
  read Large: EnumItem<"VideoSampleSize">,
  read Full: EnumItem<"VideoSampleSize">,
}
type ViewMode = {
  read None: EnumItem<"ViewMode">,
  read GeometryComplexity: EnumItem<"ViewMode">,
  read Transparent: EnumItem<"ViewMode">,
  read Decal: EnumItem<"ViewMode">,
}
type VirtualCursorMode = {
  read Default: EnumItem<"VirtualCursorMode">,
  read Disabled: EnumItem<"VirtualCursorMode">,
  read Enabled: EnumItem<"VirtualCursorMode">,
}
type VirtualInputMode = {
  read None: EnumItem<"VirtualInputMode">,
  read Recording: EnumItem<"VirtualInputMode">,
  read Playing: EnumItem<"VirtualInputMode">,
}
type VoiceChatDistanceAttenuationType = {
  read Inverse: EnumItem<"VoiceChatDistanceAttenuationType">,
  read Legacy: EnumItem<"VoiceChatDistanceAttenuationType">,
}
type VoiceChatState = {
  read Idle: EnumItem<"VoiceChatState">,
  read Joining: EnumItem<"VoiceChatState">,
  read JoiningRetry: EnumItem<"VoiceChatState">,
  read Joined: EnumItem<"VoiceChatState">,
  read Leaving: EnumItem<"VoiceChatState">,
  read Ended: EnumItem<"VoiceChatState">,
  read Failed: EnumItem<"VoiceChatState">,
}
type VoiceClientLeaveReasons = {
  read Unknown: EnumItem<"VoiceClientLeaveReasons">,
  read ClientNetworkDisconnected: EnumItem<"VoiceClientLeaveReasons">,
  read PlayerLeft: EnumItem<"VoiceClientLeaveReasons">,
  read ClientShutdown: EnumItem<"VoiceClientLeaveReasons">,
  read PublishFailed: EnumItem<"VoiceClientLeaveReasons">,
  read RejoinReceived: EnumItem<"VoiceClientLeaveReasons">,
  read VoiceReboot: EnumItem<"VoiceClientLeaveReasons">,
  read ImguiDebugLeave: EnumItem<"VoiceClientLeaveReasons">,
  read LuaInitiated: EnumItem<"VoiceClientLeaveReasons">,
}
type VoiceControlPath = {
  read Publish: EnumItem<"VoiceControlPath">,
  read Subscribe: EnumItem<"VoiceControlPath">,
  read Join: EnumItem<"VoiceControlPath">,
}
type VoiceRccReconnectReason = {
  read Unknown: EnumItem<"VoiceRccReconnectReason">,
  read Migration: EnumItem<"VoiceRccReconnectReason">,
  read CloseRoom: EnumItem<"VoiceRccReconnectReason">,
  read FAEUpdate: EnumItem<"VoiceRccReconnectReason">,
  read BlockListChanged: EnumItem<"VoiceRccReconnectReason">,
}
type VolumetricAudio = {
  read Disabled: EnumItem<"VolumetricAudio">,
  read Automatic: EnumItem<"VolumetricAudio">,
  read Enabled: EnumItem<"VolumetricAudio">,
}
type WaterDirection = {
  read NegX: EnumItem<"WaterDirection">,
  read X: EnumItem<"WaterDirection">,
  read NegY: EnumItem<"WaterDirection">,
  read Y: EnumItem<"WaterDirection">,
  read NegZ: EnumItem<"WaterDirection">,
  read Z: EnumItem<"WaterDirection">,
}
type WaterForce = {
  read None: EnumItem<"WaterForce">,
  read Small: EnumItem<"WaterForce">,
  read Medium: EnumItem<"WaterForce">,
  read Strong: EnumItem<"WaterForce">,
  read Max: EnumItem<"WaterForce">,
}
type WebSocketState = {
  read Connecting: EnumItem<"WebSocketState">,
  read Open: EnumItem<"WebSocketState">,
  read Closing: EnumItem<"WebSocketState">,
  read Closed: EnumItem<"WebSocketState">,
}
type WebStreamClientState = {
  read Connecting: EnumItem<"WebStreamClientState">,
  read Open: EnumItem<"WebStreamClientState">,
  read Error: EnumItem<"WebStreamClientState">,
  read Closed: EnumItem<"WebStreamClientState">,
}
type WebStreamClientType = {
  read SSE: EnumItem<"WebStreamClientType">,
  read RawStream: EnumItem<"WebStreamClientType">,
  read WebSocket: EnumItem<"WebStreamClientType">,
}
type WeldConstraintPreserve = {
  read All: EnumItem<"WeldConstraintPreserve">,
  read None: EnumItem<"WeldConstraintPreserve">,
  read Touching: EnumItem<"WeldConstraintPreserve">,
}
type WhenUserFirstPlayed = {
  read Unknown: EnumItem<"WhenUserFirstPlayed">,
  read Days0To30: EnumItem<"WhenUserFirstPlayed">,
  read Days31To90: EnumItem<"WhenUserFirstPlayed">,
  read Days91To180: EnumItem<"WhenUserFirstPlayed">,
  read Days181To365: EnumItem<"WhenUserFirstPlayed">,
  read Days366Plus: EnumItem<"WhenUserFirstPlayed">,
}
type WhisperChatPrivacyMode = {
  read AllUsers: EnumItem<"WhisperChatPrivacyMode">,
  read NoOne: EnumItem<"WhisperChatPrivacyMode">,
}
type WrapLayerAutoSkin = {
  read Disabled: EnumItem<"WrapLayerAutoSkin">,
  read EnabledPreserve: EnumItem<"WrapLayerAutoSkin">,
  read EnabledOverride: EnumItem<"WrapLayerAutoSkin">,
}
type WrapLayerDebugMode = {
  read None: EnumItem<"WrapLayerDebugMode">,
  read BoundCage: EnumItem<"WrapLayerDebugMode">,
  read LayerCage: EnumItem<"WrapLayerDebugMode">,
  read BoundCageAndLinks: EnumItem<"WrapLayerDebugMode">,
  read Reference: EnumItem<"WrapLayerDebugMode">,
  read Rbf: EnumItem<"WrapLayerDebugMode">,
  read OuterCage: EnumItem<"WrapLayerDebugMode">,
  read ReferenceMeshAfterMorph: EnumItem<"WrapLayerDebugMode">,
  read HSROuterDetail: EnumItem<"WrapLayerDebugMode">,
  read HSROuter: EnumItem<"WrapLayerDebugMode">,
  read HSRInner: EnumItem<"WrapLayerDebugMode">,
  read HSRInnerReverse: EnumItem<"WrapLayerDebugMode">,
  read LayerCageFittedToBase: EnumItem<"WrapLayerDebugMode">,
  read LayerCageFittedToPrev: EnumItem<"WrapLayerDebugMode">,
  read PreWrapDeformerOuterCage: EnumItem<"WrapLayerDebugMode">,
  read SkinningTransfer: EnumItem<"WrapLayerDebugMode">,
}
type WrapTargetDebugMode = {
  read None: EnumItem<"WrapTargetDebugMode">,
  read TargetCageOriginal: EnumItem<"WrapTargetDebugMode">,
  read TargetCageCompressed: EnumItem<"WrapTargetDebugMode">,
  read TargetCageInterface: EnumItem<"WrapTargetDebugMode">,
  read TargetLayerCageOriginal: EnumItem<"WrapTargetDebugMode">,
  read TargetLayerCageCompressed: EnumItem<"WrapTargetDebugMode">,
  read TargetLayerInterface: EnumItem<"WrapTargetDebugMode">,
  read Rbf: EnumItem<"WrapTargetDebugMode">,
  read OuterCageDetail: EnumItem<"WrapTargetDebugMode">,
  read PreWrapDeformerCage: EnumItem<"WrapTargetDebugMode">,
}
type ZIndexBehavior = {
  read Global: EnumItem<"ZIndexBehavior">,
  read Sibling: EnumItem<"ZIndexBehavior">,
}
type EnumStatic = {
  read AccessModifierType: AccessModifierType,
  read AccessoryType: AccessoryType,
  read ActionOnAutoResumeSync: ActionOnAutoResumeSync,
  read ActionOnStopSync: ActionOnStopSync,
  read ActionType: ActionType,
  read ActivePayerStatus: ActivePayerStatus,
  read ActuatorRelativeTo: ActuatorRelativeTo,
  read ActuatorType: ActuatorType,
  read AdAvailabilityResult: AdAvailabilityResult,
  read AdEventType: AdEventType,
  read AdFormat: AdFormat,
  read AdShape: AdShape,
  read AdTeleportMethod: AdTeleportMethod,
  read AdUIEventType: AdUIEventType,
  read AdUIType: AdUIType,
  read AdUnitStatus: AdUnitStatus,
  read AdornCullingMode: AdornCullingMode,
  read AdornShading: AdornShading,
  read AgeCheckStatus: AgeCheckStatus,
  read AlignType: AlignType,
  read AlphaMode: AlphaMode,
  read AnalyticsCustomFieldKeys: AnalyticsCustomFieldKeys,
  read AnalyticsEconomyAction: AnalyticsEconomyAction,
  read AnalyticsEconomyFlowType: AnalyticsEconomyFlowType,
  read AnalyticsEconomyTransactionType: AnalyticsEconomyTransactionType,
  read AnalyticsLogLevel: AnalyticsLogLevel,
  read AnalyticsProgressionStatus: AnalyticsProgressionStatus,
  read AnalyticsProgressionType: AnalyticsProgressionType,
  read AnimationClipFromVideoStatus: AnimationClipFromVideoStatus,
  read AnimationNodeBlend2DInputMode: AnimationNodeBlend2DInputMode,
  read AnimationNodeInterruptible: AnimationNodeInterruptible,
  read AnimationNodePhaseSync: AnimationNodePhaseSync,
  read AnimationNodePlayMode: AnimationNodePlayMode,
  read AnimationNodeTransitionType: AnimationNodeTransitionType,
  read AnimationNodeType: AnimationNodeType,
  read AnimationNodeWaitFor: AnimationNodeWaitFor,
  read AnimationPriority: AnimationPriority,
  read AnimatorRetargetingMode: AnimatorRetargetingMode,
  read AnnotationChannelContentPreference: AnnotationChannelContentPreference,
  read AnnotationEditingMode: AnnotationEditingMode,
  read AnnotationPlaceContentPreference: AnnotationPlaceContentPreference,
  read AnnotationRequestStatus: AnnotationRequestStatus,
  read AnnotationRequestType: AnnotationRequestType,
  read AntiAliasing: AntiAliasing,
  read AppLifecycleManagerState: AppLifecycleManagerState,
  read AppShellActionType: AppShellActionType,
  read AppShellFeature: AppShellFeature,
  read AppUpdateStatus: AppUpdateStatus,
  read ApplyStrokeMode: ApplyStrokeMode,
  read AspectType: AspectType,
  read AssetCreatorType: AssetCreatorType,
  read AssetFetchStatus: AssetFetchStatus,
  read AssetRepresentation: AssetRepresentation,
  read AssetType: AssetType,
  read AssetTypeVerification: AssetTypeVerification,
  read AudioApiRollout: AudioApiRollout,
  read AudioCaptureMode: AudioCaptureMode,
  read AudioChannelLayout: AudioChannelLayout,
  read AudioFilterType: AudioFilterType,
  read AudioSimulationFidelity: AudioSimulationFidelity,
  read AudioSubType: AudioSubType,
  read AudioWindowSize: AudioWindowSize,
  read AuthorityMode: AuthorityMode,
  read AutoIndentRule: AutoIndentRule,
  read AutomaticSize: AutomaticSize,
  read AvatarAssetType: AvatarAssetType,
  read AvatarChatServiceFeature: AvatarChatServiceFeature,
  read AvatarContextMenuOption: AvatarContextMenuOption,
  read AvatarGenerationError: AvatarGenerationError,
  read AvatarItemType: AvatarItemType,
  read AvatarPromptResult: AvatarPromptResult,
  read AvatarSettingsAccessoryLimitMethod: AvatarSettingsAccessoryLimitMethod,
  read AvatarSettingsAccessoryMode: AvatarSettingsAccessoryMode,
  read AvatarSettingsAnimationClipsMode: AvatarSettingsAnimationClipsMode,
  read AvatarSettingsAnimationPacksMode: AvatarSettingsAnimationPacksMode,
  read AvatarSettingsAppearanceMode: AvatarSettingsAppearanceMode,
  read AvatarSettingsBuildMode: AvatarSettingsBuildMode,
  read AvatarSettingsCharacterControllerMode: AvatarSettingsCharacterControllerMode,
  read AvatarSettingsClothingMode: AvatarSettingsClothingMode,
  read AvatarSettingsCollisionMode: AvatarSettingsCollisionMode,
  read AvatarSettingsCustomAccessoryMode: AvatarSettingsCustomAccessoryMode,
  read AvatarSettingsCustomBodyType: AvatarSettingsCustomBodyType,
  read AvatarSettingsCustomClothingMode: AvatarSettingsCustomClothingMode,
  read AvatarSettingsHitAndTouchDetectionMode: AvatarSettingsHitAndTouchDetectionMode,
  read AvatarSettingsJumpMode: AvatarSettingsJumpMode,
  read AvatarSettingsLegacyCollisionMode: AvatarSettingsLegacyCollisionMode,
  read AvatarSettingsScaleMode: AvatarSettingsScaleMode,
  read AvatarThumbnailCustomizationType: AvatarThumbnailCustomizationType,
  read AvatarUnificationMode: AvatarUnificationMode,
  read Axis: Axis,
  read BenefitType: BenefitType,
  read BinType: BinType,
  read BodyPart: BodyPart,
  read BodyPartR15: BodyPartR15,
  read BorderMode: BorderMode,
  read BorderStrokePosition: BorderStrokePosition,
  read BreakReason: BreakReason,
  read BreakpointRemoveReason: BreakpointRemoveReason,
  read BulkMoveMode: BulkMoveMode,
  read BundleType: BundleType,
  read Button: Button,
  read ButtonStyle: ButtonStyle,
  read CageType: CageType,
  read CameraMode: CameraMode,
  read CameraNavigationModel: CameraNavigationModel,
  read CameraPanMode: CameraPanMode,
  read CameraSpeedAdjustBinding: CameraSpeedAdjustBinding,
  read CameraType: CameraType,
  read CanCollaborateError: CanCollaborateError,
  read CaptureGalleryPermission: CaptureGalleryPermission,
  read CaptureType: CaptureType,
  read CatalogCategoryFilter: CatalogCategoryFilter,
  read CatalogSortAggregation: CatalogSortAggregation,
  read CatalogSortType: CatalogSortType,
  read CellBlock: CellBlock,
  read CellMaterial: CellMaterial,
  read CellOrientation: CellOrientation,
  read CenterDialogType: CenterDialogType,
  read CharacterControlMode: CharacterControlMode,
  read ChatCallbackType: ChatCallbackType,
  read ChatColor: ChatColor,
  read ChatMode: ChatMode,
  read ChatPrivacyMode: ChatPrivacyMode,
  read ChatRestrictionStatus: ChatRestrictionStatus,
  read ChatStyle: ChatStyle,
  read ChatVersion: ChatVersion,
  read ClientAnimatorThrottlingMode: ClientAnimatorThrottlingMode,
  read CloseReason: CloseReason,
  read CollaboratorStatus: CollaboratorStatus,
  read CollisionFidelity: CollisionFidelity,
  read CommandPermission: CommandPermission,
  read CompileTarget: CompileTarget,
  read CompletionAcceptanceBehavior: CompletionAcceptanceBehavior,
  read CompletionItemKind: CompletionItemKind,
  read CompletionItemTag: CompletionItemTag,
  read CompletionTriggerKind: CompletionTriggerKind,
  read CompositeValueCurveType: CompositeValueCurveType,
  read CompressionAlgorithm: CompressionAlgorithm,
  read ComputerCameraMovementMode: ComputerCameraMovementMode,
  read ComputerMovementMode: ComputerMovementMode,
  read ConfigSnapshotErrorState: ConfigSnapshotErrorState,
  read ConnectionError: ConnectionError,
  read ConnectionState: ConnectionState,
  read ContentSourceType: ContentSourceType,
  read ContextActionPriority: ContextActionPriority,
  read ContextActionResult: ContextActionResult,
  read ControlMode: ControlMode,
  read CoreGuiType: CoreGuiType,
  read CreateAssetResult: CreateAssetResult,
  read CreateContentResult: CreateContentResult,
  read CreateOutfitFailure: CreateOutfitFailure,
  read CreatorType: CreatorType,
  read CreatorTypeFilter: CreatorTypeFilter,
  read CurrencyType: CurrencyType,
  read CustomCameraMode: CustomCameraMode,
  read DataModelExtractorFileType: DataModelExtractorFileType,
  read DataStoreRequestType: DataStoreRequestType,
  read DebugBreakModeType: DebugBreakModeType,
  read DebuggerEndReason: DebuggerEndReason,
  read DebuggerExceptionBreakMode: DebuggerExceptionBreakMode,
  read DebuggerFrameType: DebuggerFrameType,
  read DebuggerPauseReason: DebuggerPauseReason,
  read DebuggerResumeType: DebuggerResumeType,
  read DebuggerStatus: DebuggerStatus,
  read DefaultScriptSyncFileType: DefaultScriptSyncFileType,
  read DevCameraOcclusionMode: DevCameraOcclusionMode,
  read DevComputerCameraMovementMode: DevComputerCameraMovementMode,
  read DevComputerMovementMode: DevComputerMovementMode,
  read DevTouchCameraMovementMode: DevTouchCameraMovementMode,
  read DevTouchMovementMode: DevTouchMovementMode,
  read DeveloperMemoryTag: DeveloperMemoryTag,
  read DeviceFeatureType: DeviceFeatureType,
  read DeviceForm: DeviceForm,
  read DeviceLevel: DeviceLevel,
  read DeviceSimulatorScalingMode: DeviceSimulatorScalingMode,
  read DeviceType: DeviceType,
  read DialogBehaviorType: DialogBehaviorType,
  read DialogPurpose: DialogPurpose,
  read DialogTone: DialogTone,
  read DigitsRigDescriptionSide: DigitsRigDescriptionSide,
  read DiscountType: DiscountType,
  read DisplayScalingMode: DisplayScalingMode,
  read DisplaySize: DisplaySize,
  read DomainType: DomainType,
  read DominantAxis: DominantAxis,
  read DraftStatusCode: DraftStatusCode,
  read DragDetectorDragStyle: DragDetectorDragStyle,
  read DragDetectorPermissionPolicy: DragDetectorPermissionPolicy,
  read DragDetectorResponseStyle: DragDetectorResponseStyle,
  read DraggerCoordinateSpace: DraggerCoordinateSpace,
  read DraggerMovementMode: DraggerMovementMode,
  read DraggingScrollBar: DraggingScrollBar,
  read EasingDirection: EasingDirection,
  read EasingStyle: EasingStyle,
  read EditableStatus: EditableStatus,
  read ElasticBehavior: ElasticBehavior,
  read EmitterPositionType: EmitterPositionType,
  read EngagementLevel: EngagementLevel,
  read EngineFolder: EngineFolder,
  read EnviromentalPhysicsThrottle: EnviromentalPhysicsThrottle,
  read ExperienceActivationStatus: ExperienceActivationStatus,
  read ExperienceAuthScope: ExperienceAuthScope,
  read ExperienceEventStatus: ExperienceEventStatus,
  read ExperienceStateCaptureSelectionMode: ExperienceStateCaptureSelectionMode,
  read ExperienceStateRecordingLoadMode: ExperienceStateRecordingLoadMode,
  read ExperienceStateRecordingLoadSourceType: ExperienceStateRecordingLoadSourceType,
  read ExperienceStateRecordingPlaybackMode: ExperienceStateRecordingPlaybackMode,
  read ExplosionType: ExplosionType,
  read ExternalEditorMode: ExternalEditorMode,
  read FACSDataLod: FACSDataLod,
  read FacialAgeEstimationResultType: FacialAgeEstimationResultType,
  read FacialAnimationStreamingState: FacialAnimationStreamingState,
  read FacsActionUnit: FacsActionUnit,
  read FeatureRestrictionAbuseVector: FeatureRestrictionAbuseVector,
  read FeedbackType: FeedbackType,
  read FieldOfViewMode: FieldOfViewMode,
  read FillDirection: FillDirection,
  read FilterErrorType: FilterErrorType,
  read FilterResult: FilterResult,
  read FilterType: FilterType,
  read FinishRecordingOperation: FinishRecordingOperation,
  read FluidFidelity: FluidFidelity,
  read FluidForces: FluidForces,
  read Font: Font,
  read FontSize: FontSize,
  read FontStyle: FontStyle,
  read FontWeight: FontWeight,
  read ForceLimitMode: ForceLimitMode,
  read FormFactor: FormFactor,
  read FrameStyle: FrameStyle,
  read FramerateManagerMode: FramerateManagerMode,
  read FriendRequestEvent: FriendRequestEvent,
  read FriendStatus: FriendStatus,
  read FunctionalTestResult: FunctionalTestResult,
  read GameAvatarType: GameAvatarType,
  read GamepadType: GamepadType,
  read GearGenreSetting: GearGenreSetting,
  read GearType: GearType,
  read Genre: Genre,
  read GradientTileMode: GradientTileMode,
  read GradientType: GradientType,
  read GraphicsMode: GraphicsMode,
  read GraphicsOptimizationMode: GraphicsOptimizationMode,
  read GroupMembershipStatus: GroupMembershipStatus,
  read GuiState: GuiState,
  read GuiType: GuiType,
  read HandlesStyle: HandlesStyle,
  read HapticEffectType: HapticEffectType,
  read HashAlgorithm: HashAlgorithm,
  read HighlightDepthMode: HighlightDepthMode,
  read HorizontalAlignment: HorizontalAlignment,
  read HoverAnimateSpeed: HoverAnimateSpeed,
  read HttpCachePolicy: HttpCachePolicy,
  read HttpCompression: HttpCompression,
  read HttpContentType: HttpContentType,
  read HttpError: HttpError,
  read HttpRequestType: HttpRequestType,
  read HumanoidCollisionType: HumanoidCollisionType,
  read HumanoidDisplayDistanceType: HumanoidDisplayDistanceType,
  read HumanoidHealthDisplayType: HumanoidHealthDisplayType,
  read HumanoidRigType: HumanoidRigType,
  read HumanoidStateType: HumanoidStateType,
  read IKCollisionsMode: IKCollisionsMode,
  read IKControlConstraintSupport: IKControlConstraintSupport,
  read IKControlType: IKControlType,
  read IXPLoadingStatus: IXPLoadingStatus,
  read ImageAlphaType: ImageAlphaType,
  read ImageCombineType: ImageCombineType,
  read InOut: InOut,
  read InfoType: InfoType,
  read InitialDockState: InitialDockState,
  read InputActionType: InputActionType,
  read InputBindingType: InputBindingType,
  read InputSink: InputSink,
  read InputType: InputType,
  read InstanceFileSyncStatus: InstanceFileSyncStatus,
  read IntermediateMeshGenerationResult: IntermediateMeshGenerationResult,
  read InternalVideoUsage: InternalVideoUsage,
  read InterpolationThrottlingMode: InterpolationThrottlingMode,
  read InviteState: InviteState,
  read ItemLineAlignment: ItemLineAlignment,
  read JoinSource: JoinSource,
  read JointCreationMode: JointCreationMode,
  read KeyCode: KeyCode,
  read KeyCodeStringFormat: KeyCodeStringFormat,
  read KeyInterpolationMode: KeyInterpolationMode,
  read KeywordFilterType: KeywordFilterType,
  read Language: Language,
  read LeftRight: LeftRight,
  read LexemeType: LexemeType,
  read LightingStyle: LightingStyle,
  read Limb: Limb,
  read LineJoinMode: LineJoinMode,
  read ListDisplayMode: ListDisplayMode,
  read ListenerLocation: ListenerLocation,
  read ListenerPositionType: ListenerPositionType,
  read ListenerType: ListenerType,
  read LiveEditingAtomicUpdateResponse: LiveEditingAtomicUpdateResponse,
  read LiveEditingBroadcastMessageType: LiveEditingBroadcastMessageType,
  read LoadCharacterLayeredClothing: LoadCharacterLayeredClothing,
  read LoadDynamicHeads: LoadDynamicHeads,
  read LocationType: LocationType,
  read LuauTypeCheckMode: LuauTypeCheckMode,
  read MakeupType: MakeupType,
  read MarketplaceBulkPurchasePromptStatus: MarketplaceBulkPurchasePromptStatus,
  read MarketplaceItemPurchaseStatus: MarketplaceItemPurchaseStatus,
  read MarketplaceProductType: MarketplaceProductType,
  read MarkupKind: MarkupKind,
  read MatchmakingType: MatchmakingType,
  read Material: Material,
  read MaterialPattern: MaterialPattern,
  read MembershipType: MembershipType,
  read MeshPartDetailLevel: MeshPartDetailLevel,
  read MeshPartHeadsAndAccessories: MeshPartHeadsAndAccessories,
  read MeshScaleUnit: MeshScaleUnit,
  read MeshType: MeshType,
  read MessageType: MessageType,
  read ModelLevelOfDetail: ModelLevelOfDetail,
  read ModelStreamingBehavior: ModelStreamingBehavior,
  read ModelStreamingMode: ModelStreamingMode,
  read ModerationResultCategory: ModerationResultCategory,
  read ModerationResultLabel: ModerationResultLabel,
  read ModerationStatus: ModerationStatus,
  read ModifierKey: ModifierKey,
  read MouseBehavior: MouseBehavior,
  read MoveState: MoveState,
  read MuteState: MuteState,
  read NameOcclusion: NameOcclusion,
  read NegateOperationHiddenHistory: NegateOperationHiddenHistory,
  read NetworkOwnership: NetworkOwnership,
  read NetworkStatus: NetworkStatus,
  read NoiseType: NoiseType,
  read NormalId: NormalId,
  read NotificationButtonType: NotificationButtonType,
  read OperationType: OperationType,
  read OrientationAlignmentMode: OrientationAlignmentMode,
  read OutfitSource: OutfitSource,
  read OutfitType: OutfitType,
  read OutputLayoutMode: OutputLayoutMode,
  read OverrideMouseIconBehavior: OverrideMouseIconBehavior,
  read PackagePermission: PackagePermission,
  read PartType: PartType,
  read ParticleEmitterShape: ParticleEmitterShape,
  read ParticleEmitterShapeInOut: ParticleEmitterShapeInOut,
  read ParticleEmitterShapeStyle: ParticleEmitterShapeStyle,
  read ParticleFlipbookLayout: ParticleFlipbookLayout,
  read ParticleFlipbookMode: ParticleFlipbookMode,
  read ParticleFlipbookTextureCompatible: ParticleFlipbookTextureCompatible,
  read ParticleOrientation: ParticleOrientation,
  read PathStatus: PathStatus,
  read PathWaypointAction: PathWaypointAction,
  read PathfindingUseImprovedSearch: PathfindingUseImprovedSearch,
  read PeoplePageLayout: PeoplePageLayout,
  read PerformanceOverlayMode: PerformanceOverlayMode,
  read PermissionLevelShown: PermissionLevelShown,
  read PhysicalConstraintType: PhysicalConstraintType,
  read PhysicsSimulationRate: PhysicsSimulationRate,
  read PhysicsSteppingMethod: PhysicsSteppingMethod,
  read PlaceContentPreference: PlaceContentPreference,
  read PlacePublishType: PlacePublishType,
  read Platform: Platform,
  read PlaybackState: PlaybackState,
  read PlayerActions: PlayerActions,
  read PlayerCharacterDestroyBehavior: PlayerCharacterDestroyBehavior,
  read PlayerChatType: PlayerChatType,
  read PlayerDataErrorState: PlayerDataErrorState,
  read PlayerDataLoadFailureBehavior: PlayerDataLoadFailureBehavior,
  read PlayerExitReason: PlayerExitReason,
  read PlayerPlatformActivationStatus: PlayerPlatformActivationStatus,
  read PlayerPlatformSpenderStatus: PlayerPlatformSpenderStatus,
  read PluginConnectionTargetType: PluginConnectionTargetType,
  read PoseEasingDirection: PoseEasingDirection,
  read PoseEasingStyle: PoseEasingStyle,
  read PositionAlignmentMode: PositionAlignmentMode,
  read PredictionMode: PredictionMode,
  read PredictionStatus: PredictionStatus,
  read PredictiveStreamingMode: PredictiveStreamingMode,
  read PreferredInput: PreferredInput,
  read PreferredTextSize: PreferredTextSize,
  read PrefetchDownloadStatus: PrefetchDownloadStatus,
  read PrimalPhysicsSolver: PrimalPhysicsSolver,
  read PrimitiveType: PrimitiveType,
  read PrivilegeType: PrivilegeType,
  read ProductLocationRestriction: ProductLocationRestriction,
  read ProductPurchaseChannel: ProductPurchaseChannel,
  read ProductPurchaseDecision: ProductPurchaseDecision,
  read PromptCreateAssetResult: PromptCreateAssetResult,
  read PromptCreateAvatarResult: PromptCreateAvatarResult,
  read PromptCreateOutfitResult: PromptCreateOutfitResult,
  read PromptExperienceDetailsResult: PromptExperienceDetailsResult,
  read PromptLinkSharingResult: PromptLinkSharingResult,
  read PromptPublishAssetResult: PromptPublishAssetResult,
  read PropertyStatus: PropertyStatus,
  read ProximityPromptExclusivity: ProximityPromptExclusivity,
  read ProximityPromptInputType: ProximityPromptInputType,
  read ProximityPromptStyle: ProximityPromptStyle,
  read PurchaseOption: PurchaseOption,
  read QualityLevel: QualityLevel,
  read R15CollisionType: R15CollisionType,
  read RaycastFilterType: RaycastFilterType,
  read ReadCapturesFromGalleryResult: ReadCapturesFromGalleryResult,
  read ReceiptDecision: ReceiptDecision,
  read ReceiptType: ReceiptType,
  read RecommendationActionType: RecommendationActionType,
  read RecommendationDepartureIntent: RecommendationDepartureIntent,
  read RecommendationImpressionType: RecommendationImpressionType,
  read RecommendationItemContentType: RecommendationItemContentType,
  read RecommendationItemVisibility: RecommendationItemVisibility,
  read RecommendationPreferenceTargetType: RecommendationPreferenceTargetType,
  read RecommendationPreferenceType: RecommendationPreferenceType,
  read RejectCharacterDeletions: RejectCharacterDeletions,
  read RenderFidelity: RenderFidelity,
  read RenderPriority: RenderPriority,
  read RenderingCacheOptimizationMode: RenderingCacheOptimizationMode,
  read RenderingTestComparisonMethod: RenderingTestComparisonMethod,
  read ReplicateInstanceDestroySetting: ReplicateInstanceDestroySetting,
  read ResamplerMode: ResamplerMode,
  read ReservedHighlightId: ReservedHighlightId,
  read RestPose: RestPose,
  read RestPoseModel: RestPoseModel,
  read ReturnKeyType: ReturnKeyType,
  read ReverbType: ReverbType,
  read ReviewableContentState: ReviewableContentState,
  read RibbonTool: RibbonTool,
  read RigLabel: RigLabel,
  read RigScale: RigScale,
  read RigType: RigType,
  read RollOffMode: RollOffMode,
  read RolloutState: RolloutState,
  read RotationOrder: RotationOrder,
  read RotationType: RotationType,
  read RsvpStatus: RsvpStatus,
  read RtlTextSupport: RtlTextSupport,
  read RunContext: RunContext,
  read RunState: RunState,
  read RuntimeUndoBehavior: RuntimeUndoBehavior,
  read SafeAreaCompatibility: SafeAreaCompatibility,
  read SalesTypeFilter: SalesTypeFilter,
  read SandboxedInstanceMode: SandboxedInstanceMode,
  read SaveAvatarThumbnailCustomizationFailure: SaveAvatarThumbnailCustomizationFailure,
  read SaveFilter: SaveFilter,
  read SavedQualitySetting: SavedQualitySetting,
  read ScaleType: ScaleType,
  read ScopeCheckResult: ScopeCheckResult,
  read ScreenInsets: ScreenInsets,
  read ScreenOrientation: ScreenOrientation,
  read ScreenshotCaptureResult: ScreenshotCaptureResult,
  read ScriptStoppedReason: ScriptStoppedReason,
  read ScriptVariableScope: ScriptVariableScope,
  read ScrollBarInset: ScrollBarInset,
  read ScrollingDirection: ScrollingDirection,
  read SecurityCapability: SecurityCapability,
  read SelectionBehavior: SelectionBehavior,
  read SelectionRenderMode: SelectionRenderMode,
  read SelfViewPosition: SelfViewPosition,
  read SensorMode: SensorMode,
  read SensorUpdateType: SensorUpdateType,
  read ServerLiveEditingMode: ServerLiveEditingMode,
  read ServiceVisibility: ServiceVisibility,
  read Severity: Severity,
  read ShowAdResult: ShowAdResult,
  read SignalBehavior: SignalBehavior,
  read SimulationMode: SimulationMode,
  read SizeConstraint: SizeConstraint,
  read SlimTintMode: SlimTintMode,
  read SolidPrimitiveType: SolidPrimitiveType,
  read SolverConvergenceMetricType: SolverConvergenceMetricType,
  read SolverConvergenceVisualizationMode: SolverConvergenceVisualizationMode,
  read SortDirection: SortDirection,
  read SortOrder: SortOrder,
  read SpecialKey: SpecialKey,
  read StartCorner: StartCorner,
  read StateObjectFieldType: StateObjectFieldType,
  read Status: Status,
  read StepFrequency: StepFrequency,
  read StreamOutBehavior: StreamOutBehavior,
  read StreamingIntegrityMode: StreamingIntegrityMode,
  read StreamingPauseMode: StreamingPauseMode,
  read StrokeSizingMode: StrokeSizingMode,
  read StudioAction: StudioAction,
  read StudioCaptureBufferStatus: StudioCaptureBufferStatus,
  read StudioCaptureScreenshotFormat: StudioCaptureScreenshotFormat,
  read StudioCloseMode: StudioCloseMode,
  read StudioDataModelType: StudioDataModelType,
  read StudioPlaceUpdateFailureReason: StudioPlaceUpdateFailureReason,
  read StudioScriptEditorColorCategories: StudioScriptEditorColorCategories,
  read StudioScriptEditorColorPresets: StudioScriptEditorColorPresets,
  read StudioStyleGuideColor: StudioStyleGuideColor,
  read StudioStyleGuideModifier: StudioStyleGuideModifier,
  read Style: Style,
  read SubscriptionExpirationReason: SubscriptionExpirationReason,
  read SubscriptionPaymentStatus: SubscriptionPaymentStatus,
  read SubscriptionPeriod: SubscriptionPeriod,
  read SubscriptionState: SubscriptionState,
  read SurfaceConstraint: SurfaceConstraint,
  read SurfaceGuiShape: SurfaceGuiShape,
  read SurfaceGuiSizingMode: SurfaceGuiSizingMode,
  read SurfaceType: SurfaceType,
  read SwipeDirection: SwipeDirection,
  read SystemThemeValue: SystemThemeValue,
  read TableMajorAxis: TableMajorAxis,
  read TagReplicability: TagReplicability,
  read TeamCreateErrorState: TeamCreateErrorState,
  read Technology: Technology,
  read TelemetryBackend: TelemetryBackend,
  read TelemetryStandardizedField: TelemetryStandardizedField,
  read TeleportMethod: TeleportMethod,
  read TeleportResult: TeleportResult,
  read TeleportState: TeleportState,
  read TeleportType: TeleportType,
  read TerrainAcquisitionMethod: TerrainAcquisitionMethod,
  read TerrainFace: TerrainFace,
  read TerrainLiquidMergeOperation: TerrainLiquidMergeOperation,
  read TerrainSolidMergeOperation: TerrainSolidMergeOperation,
  read TextChatMessageStatus: TextChatMessageStatus,
  read TextDirection: TextDirection,
  read TextFilterContext: TextFilterContext,
  read TextInputType: TextInputType,
  read TextTruncate: TextTruncate,
  read TextXAlignment: TextXAlignment,
  read TextYAlignment: TextYAlignment,
  read TextureMode: TextureMode,
  read TextureQueryType: TextureQueryType,
  read ThreadPoolConfig: ThreadPoolConfig,
  read ThrottlingPriority: ThrottlingPriority,
  read ThumbnailSize: ThumbnailSize,
  read ThumbnailType: ThumbnailType,
  read TickCountSampleMethod: TickCountSampleMethod,
  read TonemapperPreset: TonemapperPreset,
  read TopBottom: TopBottom,
  read TouchCameraMovementMode: TouchCameraMovementMode,
  read TouchMovementMode: TouchMovementMode,
  read TrackerError: TrackerError,
  read TrackerExtrapolationFlagMode: TrackerExtrapolationFlagMode,
  read TrackerFaceTrackingStatus: TrackerFaceTrackingStatus,
  read TrackerLodFlagMode: TrackerLodFlagMode,
  read TrackerLodValueMode: TrackerLodValueMode,
  read TrackerMode: TrackerMode,
  read TrackerPromptEvent: TrackerPromptEvent,
  read TrackerType: TrackerType,
  read TriStateBoolean: TriStateBoolean,
  read TweenStatus: TweenStatus,
  read UICaptureMode: UICaptureMode,
  read UIDragDetectorBoundingBehavior: UIDragDetectorBoundingBehavior,
  read UIDragDetectorDragRelativity: UIDragDetectorDragRelativity,
  read UIDragDetectorDragSpace: UIDragDetectorDragSpace,
  read UIDragDetectorDragStyle: UIDragDetectorDragStyle,
  read UIDragDetectorResponseStyle: UIDragDetectorResponseStyle,
  read UIDragSpeedAxisMapping: UIDragSpeedAxisMapping,
  read UIFlexAlignment: UIFlexAlignment,
  read UIFlexMode: UIFlexMode,
  read UITheme: UITheme,
  read UiMessageType: UiMessageType,
  read UpdateState: UpdateState,
  read UploadCaptureResult: UploadCaptureResult,
  read UsageContext: UsageContext,
  read UserCFrame: UserCFrame,
  read UserInputState: UserInputState,
  read UserInputType: UserInputType,
  read VRComfortSetting: VRComfortSetting,
  read VRControllerModelMode: VRControllerModelMode,
  read VRDeviceType: VRDeviceType,
  read VRLaserPointerMode: VRLaserPointerMode,
  read VRSafetyBubbleMode: VRSafetyBubbleMode,
  read VRScaling: VRScaling,
  read VRSessionState: VRSessionState,
  read VRTouchpad: VRTouchpad,
  read VRTouchpadMode: VRTouchpadMode,
  read VelocityConstraintMode: VelocityConstraintMode,
  read VerticalAlignment: VerticalAlignment,
  read VerticalScrollBarPosition: VerticalScrollBarPosition,
  read VibrationMotor: VibrationMotor,
  read VideoCaptureResult: VideoCaptureResult,
  read VideoCaptureStartedResult: VideoCaptureStartedResult,
  read VideoDeviceCaptureQuality: VideoDeviceCaptureQuality,
  read VideoError: VideoError,
  read VideoSampleSize: VideoSampleSize,
  read ViewMode: ViewMode,
  read VirtualCursorMode: VirtualCursorMode,
  read VirtualInputMode: VirtualInputMode,
  read VoiceChatDistanceAttenuationType: VoiceChatDistanceAttenuationType,
  read VoiceChatState: VoiceChatState,
  read VoiceClientLeaveReasons: VoiceClientLeaveReasons,
  read VoiceControlPath: VoiceControlPath,
  read VoiceRccReconnectReason: VoiceRccReconnectReason,
  read VolumetricAudio: VolumetricAudio,
  read WaterDirection: WaterDirection,
  read WaterForce: WaterForce,
  read WebSocketState: WebSocketState,
  read WebStreamClientState: WebStreamClientState,
  read WebStreamClientType: WebStreamClientType,
  read WeldConstraintPreserve: WeldConstraintPreserve,
  read WhenUserFirstPlayed: WhenUserFirstPlayed,
  read WhisperChatPrivacyMode: WhisperChatPrivacyMode,
  read WrapLayerAutoSkin: WrapLayerAutoSkin,
  read WrapLayerDebugMode: WrapLayerDebugMode,
  read WrapTargetDebugMode: WrapTargetDebugMode,
  read ZIndexBehavior: ZIndexBehavior,
}
type Enum = {
  read AccessModifierType: EnumItem<"AccessModifierType">,
  read AccessoryType: EnumItem<"AccessoryType">,
  read ActionOnAutoResumeSync: EnumItem<"ActionOnAutoResumeSync">,
  read ActionOnStopSync: EnumItem<"ActionOnStopSync">,
  read ActionType: EnumItem<"ActionType">,
  read ActivePayerStatus: EnumItem<"ActivePayerStatus">,
  read ActuatorRelativeTo: EnumItem<"ActuatorRelativeTo">,
  read ActuatorType: EnumItem<"ActuatorType">,
  read AdAvailabilityResult: EnumItem<"AdAvailabilityResult">,
  read AdEventType: EnumItem<"AdEventType">,
  read AdFormat: EnumItem<"AdFormat">,
  read AdShape: EnumItem<"AdShape">,
  read AdTeleportMethod: EnumItem<"AdTeleportMethod">,
  read AdUIEventType: EnumItem<"AdUIEventType">,
  read AdUIType: EnumItem<"AdUIType">,
  read AdUnitStatus: EnumItem<"AdUnitStatus">,
  read AdornCullingMode: EnumItem<"AdornCullingMode">,
  read AdornShading: EnumItem<"AdornShading">,
  read AgeCheckStatus: EnumItem<"AgeCheckStatus">,
  read AlignType: EnumItem<"AlignType">,
  read AlphaMode: EnumItem<"AlphaMode">,
  read AnalyticsCustomFieldKeys: EnumItem<"AnalyticsCustomFieldKeys">,
  read AnalyticsEconomyAction: EnumItem<"AnalyticsEconomyAction">,
  read AnalyticsEconomyFlowType: EnumItem<"AnalyticsEconomyFlowType">,
  read AnalyticsEconomyTransactionType: EnumItem<"AnalyticsEconomyTransactionType">,
  read AnalyticsLogLevel: EnumItem<"AnalyticsLogLevel">,
  read AnalyticsProgressionStatus: EnumItem<"AnalyticsProgressionStatus">,
  read AnalyticsProgressionType: EnumItem<"AnalyticsProgressionType">,
  read AnimationClipFromVideoStatus: EnumItem<"AnimationClipFromVideoStatus">,
  read AnimationNodeBlend2DInputMode: EnumItem<"AnimationNodeBlend2DInputMode">,
  read AnimationNodeInterruptible: EnumItem<"AnimationNodeInterruptible">,
  read AnimationNodePhaseSync: EnumItem<"AnimationNodePhaseSync">,
  read AnimationNodePlayMode: EnumItem<"AnimationNodePlayMode">,
  read AnimationNodeTransitionType: EnumItem<"AnimationNodeTransitionType">,
  read AnimationNodeType: EnumItem<"AnimationNodeType">,
  read AnimationNodeWaitFor: EnumItem<"AnimationNodeWaitFor">,
  read AnimationPriority: EnumItem<"AnimationPriority">,
  read AnimatorRetargetingMode: EnumItem<"AnimatorRetargetingMode">,
  read AnnotationChannelContentPreference: EnumItem<"AnnotationChannelContentPreference">,
  read AnnotationEditingMode: EnumItem<"AnnotationEditingMode">,
  read AnnotationPlaceContentPreference: EnumItem<"AnnotationPlaceContentPreference">,
  read AnnotationRequestStatus: EnumItem<"AnnotationRequestStatus">,
  read AnnotationRequestType: EnumItem<"AnnotationRequestType">,
  read AntiAliasing: EnumItem<"AntiAliasing">,
  read AppLifecycleManagerState: EnumItem<"AppLifecycleManagerState">,
  read AppShellActionType: EnumItem<"AppShellActionType">,
  read AppShellFeature: EnumItem<"AppShellFeature">,
  read AppUpdateStatus: EnumItem<"AppUpdateStatus">,
  read ApplyStrokeMode: EnumItem<"ApplyStrokeMode">,
  read AspectType: EnumItem<"AspectType">,
  read AssetCreatorType: EnumItem<"AssetCreatorType">,
  read AssetFetchStatus: EnumItem<"AssetFetchStatus">,
  read AssetRepresentation: EnumItem<"AssetRepresentation">,
  read AssetType: EnumItem<"AssetType">,
  read AssetTypeVerification: EnumItem<"AssetTypeVerification">,
  read AudioApiRollout: EnumItem<"AudioApiRollout">,
  read AudioCaptureMode: EnumItem<"AudioCaptureMode">,
  read AudioChannelLayout: EnumItem<"AudioChannelLayout">,
  read AudioFilterType: EnumItem<"AudioFilterType">,
  read AudioSimulationFidelity: EnumItem<"AudioSimulationFidelity">,
  read AudioSubType: EnumItem<"AudioSubType">,
  read AudioWindowSize: EnumItem<"AudioWindowSize">,
  read AuthorityMode: EnumItem<"AuthorityMode">,
  read AutoIndentRule: EnumItem<"AutoIndentRule">,
  read AutomaticSize: EnumItem<"AutomaticSize">,
  read AvatarAssetType: EnumItem<"AvatarAssetType">,
  read AvatarChatServiceFeature: EnumItem<"AvatarChatServiceFeature">,
  read AvatarContextMenuOption: EnumItem<"AvatarContextMenuOption">,
  read AvatarGenerationError: EnumItem<"AvatarGenerationError">,
  read AvatarItemType: EnumItem<"AvatarItemType">,
  read AvatarPromptResult: EnumItem<"AvatarPromptResult">,
  read AvatarSettingsAccessoryLimitMethod: EnumItem<"AvatarSettingsAccessoryLimitMethod">,
  read AvatarSettingsAccessoryMode: EnumItem<"AvatarSettingsAccessoryMode">,
  read AvatarSettingsAnimationClipsMode: EnumItem<"AvatarSettingsAnimationClipsMode">,
  read AvatarSettingsAnimationPacksMode: EnumItem<"AvatarSettingsAnimationPacksMode">,
  read AvatarSettingsAppearanceMode: EnumItem<"AvatarSettingsAppearanceMode">,
  read AvatarSettingsBuildMode: EnumItem<"AvatarSettingsBuildMode">,
  read AvatarSettingsCharacterControllerMode: EnumItem<"AvatarSettingsCharacterControllerMode">,
  read AvatarSettingsClothingMode: EnumItem<"AvatarSettingsClothingMode">,
  read AvatarSettingsCollisionMode: EnumItem<"AvatarSettingsCollisionMode">,
  read AvatarSettingsCustomAccessoryMode: EnumItem<"AvatarSettingsCustomAccessoryMode">,
  read AvatarSettingsCustomBodyType: EnumItem<"AvatarSettingsCustomBodyType">,
  read AvatarSettingsCustomClothingMode: EnumItem<"AvatarSettingsCustomClothingMode">,
  read AvatarSettingsHitAndTouchDetectionMode: EnumItem<"AvatarSettingsHitAndTouchDetectionMode">,
  read AvatarSettingsJumpMode: EnumItem<"AvatarSettingsJumpMode">,
  read AvatarSettingsLegacyCollisionMode: EnumItem<"AvatarSettingsLegacyCollisionMode">,
  read AvatarSettingsScaleMode: EnumItem<"AvatarSettingsScaleMode">,
  read AvatarThumbnailCustomizationType: EnumItem<"AvatarThumbnailCustomizationType">,
  read AvatarUnificationMode: EnumItem<"AvatarUnificationMode">,
  read Axis: EnumItem<"Axis">,
  read BenefitType: EnumItem<"BenefitType">,
  read BinType: EnumItem<"BinType">,
  read BodyPart: EnumItem<"BodyPart">,
  read BodyPartR15: EnumItem<"BodyPartR15">,
  read BorderMode: EnumItem<"BorderMode">,
  read BorderStrokePosition: EnumItem<"BorderStrokePosition">,
  read BreakReason: EnumItem<"BreakReason">,
  read BreakpointRemoveReason: EnumItem<"BreakpointRemoveReason">,
  read BulkMoveMode: EnumItem<"BulkMoveMode">,
  read BundleType: EnumItem<"BundleType">,
  read Button: EnumItem<"Button">,
  read ButtonStyle: EnumItem<"ButtonStyle">,
  read CageType: EnumItem<"CageType">,
  read CameraMode: EnumItem<"CameraMode">,
  read CameraNavigationModel: EnumItem<"CameraNavigationModel">,
  read CameraPanMode: EnumItem<"CameraPanMode">,
  read CameraSpeedAdjustBinding: EnumItem<"CameraSpeedAdjustBinding">,
  read CameraType: EnumItem<"CameraType">,
  read CanCollaborateError: EnumItem<"CanCollaborateError">,
  read CaptureGalleryPermission: EnumItem<"CaptureGalleryPermission">,
  read CaptureType: EnumItem<"CaptureType">,
  read CatalogCategoryFilter: EnumItem<"CatalogCategoryFilter">,
  read CatalogSortAggregation: EnumItem<"CatalogSortAggregation">,
  read CatalogSortType: EnumItem<"CatalogSortType">,
  read CellBlock: EnumItem<"CellBlock">,
  read CellMaterial: EnumItem<"CellMaterial">,
  read CellOrientation: EnumItem<"CellOrientation">,
  read CenterDialogType: EnumItem<"CenterDialogType">,
  read CharacterControlMode: EnumItem<"CharacterControlMode">,
  read ChatCallbackType: EnumItem<"ChatCallbackType">,
  read ChatColor: EnumItem<"ChatColor">,
  read ChatMode: EnumItem<"ChatMode">,
  read ChatPrivacyMode: EnumItem<"ChatPrivacyMode">,
  read ChatRestrictionStatus: EnumItem<"ChatRestrictionStatus">,
  read ChatStyle: EnumItem<"ChatStyle">,
  read ChatVersion: EnumItem<"ChatVersion">,
  read ClientAnimatorThrottlingMode: EnumItem<"ClientAnimatorThrottlingMode">,
  read CloseReason: EnumItem<"CloseReason">,
  read CollaboratorStatus: EnumItem<"CollaboratorStatus">,
  read CollisionFidelity: EnumItem<"CollisionFidelity">,
  read CommandPermission: EnumItem<"CommandPermission">,
  read CompileTarget: EnumItem<"CompileTarget">,
  read CompletionAcceptanceBehavior: EnumItem<"CompletionAcceptanceBehavior">,
  read CompletionItemKind: EnumItem<"CompletionItemKind">,
  read CompletionItemTag: EnumItem<"CompletionItemTag">,
  read CompletionTriggerKind: EnumItem<"CompletionTriggerKind">,
  read CompositeValueCurveType: EnumItem<"CompositeValueCurveType">,
  read CompressionAlgorithm: EnumItem<"CompressionAlgorithm">,
  read ComputerCameraMovementMode: EnumItem<"ComputerCameraMovementMode">,
  read ComputerMovementMode: EnumItem<"ComputerMovementMode">,
  read ConfigSnapshotErrorState: EnumItem<"ConfigSnapshotErrorState">,
  read ConnectionError: EnumItem<"ConnectionError">,
  read ConnectionState: EnumItem<"ConnectionState">,
  read ContentSourceType: EnumItem<"ContentSourceType">,
  read ContextActionPriority: EnumItem<"ContextActionPriority">,
  read ContextActionResult: EnumItem<"ContextActionResult">,
  read ControlMode: EnumItem<"ControlMode">,
  read CoreGuiType: EnumItem<"CoreGuiType">,
  read CreateAssetResult: EnumItem<"CreateAssetResult">,
  read CreateContentResult: EnumItem<"CreateContentResult">,
  read CreateOutfitFailure: EnumItem<"CreateOutfitFailure">,
  read CreatorType: EnumItem<"CreatorType">,
  read CreatorTypeFilter: EnumItem<"CreatorTypeFilter">,
  read CurrencyType: EnumItem<"CurrencyType">,
  read CustomCameraMode: EnumItem<"CustomCameraMode">,
  read DataModelExtractorFileType: EnumItem<"DataModelExtractorFileType">,
  read DataStoreRequestType: EnumItem<"DataStoreRequestType">,
  read DebugBreakModeType: EnumItem<"DebugBreakModeType">,
  read DebuggerEndReason: EnumItem<"DebuggerEndReason">,
  read DebuggerExceptionBreakMode: EnumItem<"DebuggerExceptionBreakMode">,
  read DebuggerFrameType: EnumItem<"DebuggerFrameType">,
  read DebuggerPauseReason: EnumItem<"DebuggerPauseReason">,
  read DebuggerResumeType: EnumItem<"DebuggerResumeType">,
  read DebuggerStatus: EnumItem<"DebuggerStatus">,
  read DefaultScriptSyncFileType: EnumItem<"DefaultScriptSyncFileType">,
  read DevCameraOcclusionMode: EnumItem<"DevCameraOcclusionMode">,
  read DevComputerCameraMovementMode: EnumItem<"DevComputerCameraMovementMode">,
  read DevComputerMovementMode: EnumItem<"DevComputerMovementMode">,
  read DevTouchCameraMovementMode: EnumItem<"DevTouchCameraMovementMode">,
  read DevTouchMovementMode: EnumItem<"DevTouchMovementMode">,
  read DeveloperMemoryTag: EnumItem<"DeveloperMemoryTag">,
  read DeviceFeatureType: EnumItem<"DeviceFeatureType">,
  read DeviceForm: EnumItem<"DeviceForm">,
  read DeviceLevel: EnumItem<"DeviceLevel">,
  read DeviceSimulatorScalingMode: EnumItem<"DeviceSimulatorScalingMode">,
  read DeviceType: EnumItem<"DeviceType">,
  read DialogBehaviorType: EnumItem<"DialogBehaviorType">,
  read DialogPurpose: EnumItem<"DialogPurpose">,
  read DialogTone: EnumItem<"DialogTone">,
  read DigitsRigDescriptionSide: EnumItem<"DigitsRigDescriptionSide">,
  read DiscountType: EnumItem<"DiscountType">,
  read DisplayScalingMode: EnumItem<"DisplayScalingMode">,
  read DisplaySize: EnumItem<"DisplaySize">,
  read DomainType: EnumItem<"DomainType">,
  read DominantAxis: EnumItem<"DominantAxis">,
  read DraftStatusCode: EnumItem<"DraftStatusCode">,
  read DragDetectorDragStyle: EnumItem<"DragDetectorDragStyle">,
  read DragDetectorPermissionPolicy: EnumItem<"DragDetectorPermissionPolicy">,
  read DragDetectorResponseStyle: EnumItem<"DragDetectorResponseStyle">,
  read DraggerCoordinateSpace: EnumItem<"DraggerCoordinateSpace">,
  read DraggerMovementMode: EnumItem<"DraggerMovementMode">,
  read DraggingScrollBar: EnumItem<"DraggingScrollBar">,
  read EasingDirection: EnumItem<"EasingDirection">,
  read EasingStyle: EnumItem<"EasingStyle">,
  read EditableStatus: EnumItem<"EditableStatus">,
  read ElasticBehavior: EnumItem<"ElasticBehavior">,
  read EmitterPositionType: EnumItem<"EmitterPositionType">,
  read EngagementLevel: EnumItem<"EngagementLevel">,
  read EngineFolder: EnumItem<"EngineFolder">,
  read EnviromentalPhysicsThrottle: EnumItem<"EnviromentalPhysicsThrottle">,
  read ExperienceActivationStatus: EnumItem<"ExperienceActivationStatus">,
  read ExperienceAuthScope: EnumItem<"ExperienceAuthScope">,
  read ExperienceEventStatus: EnumItem<"ExperienceEventStatus">,
  read ExperienceStateCaptureSelectionMode: EnumItem<"ExperienceStateCaptureSelectionMode">,
  read ExperienceStateRecordingLoadMode: EnumItem<"ExperienceStateRecordingLoadMode">,
  read ExperienceStateRecordingLoadSourceType: EnumItem<"ExperienceStateRecordingLoadSourceType">,
  read ExperienceStateRecordingPlaybackMode: EnumItem<"ExperienceStateRecordingPlaybackMode">,
  read ExplosionType: EnumItem<"ExplosionType">,
  read ExternalEditorMode: EnumItem<"ExternalEditorMode">,
  read FACSDataLod: EnumItem<"FACSDataLod">,
  read FacialAgeEstimationResultType: EnumItem<"FacialAgeEstimationResultType">,
  read FacialAnimationStreamingState: EnumItem<"FacialAnimationStreamingState">,
  read FacsActionUnit: EnumItem<"FacsActionUnit">,
  read FeatureRestrictionAbuseVector: EnumItem<"FeatureRestrictionAbuseVector">,
  read FeedbackType: EnumItem<"FeedbackType">,
  read FieldOfViewMode: EnumItem<"FieldOfViewMode">,
  read FillDirection: EnumItem<"FillDirection">,
  read FilterErrorType: EnumItem<"FilterErrorType">,
  read FilterResult: EnumItem<"FilterResult">,
  read FilterType: EnumItem<"FilterType">,
  read FinishRecordingOperation: EnumItem<"FinishRecordingOperation">,
  read FluidFidelity: EnumItem<"FluidFidelity">,
  read FluidForces: EnumItem<"FluidForces">,
  read Font: EnumItem<"Font">,
  read FontSize: EnumItem<"FontSize">,
  read FontStyle: EnumItem<"FontStyle">,
  read FontWeight: EnumItem<"FontWeight">,
  read ForceLimitMode: EnumItem<"ForceLimitMode">,
  read FormFactor: EnumItem<"FormFactor">,
  read FrameStyle: EnumItem<"FrameStyle">,
  read FramerateManagerMode: EnumItem<"FramerateManagerMode">,
  read FriendRequestEvent: EnumItem<"FriendRequestEvent">,
  read FriendStatus: EnumItem<"FriendStatus">,
  read FunctionalTestResult: EnumItem<"FunctionalTestResult">,
  read GameAvatarType: EnumItem<"GameAvatarType">,
  read GamepadType: EnumItem<"GamepadType">,
  read GearGenreSetting: EnumItem<"GearGenreSetting">,
  read GearType: EnumItem<"GearType">,
  read Genre: EnumItem<"Genre">,
  read GradientTileMode: EnumItem<"GradientTileMode">,
  read GradientType: EnumItem<"GradientType">,
  read GraphicsMode: EnumItem<"GraphicsMode">,
  read GraphicsOptimizationMode: EnumItem<"GraphicsOptimizationMode">,
  read GroupMembershipStatus: EnumItem<"GroupMembershipStatus">,
  read GuiState: EnumItem<"GuiState">,
  read GuiType: EnumItem<"GuiType">,
  read HandlesStyle: EnumItem<"HandlesStyle">,
  read HapticEffectType: EnumItem<"HapticEffectType">,
  read HashAlgorithm: EnumItem<"HashAlgorithm">,
  read HighlightDepthMode: EnumItem<"HighlightDepthMode">,
  read HorizontalAlignment: EnumItem<"HorizontalAlignment">,
  read HoverAnimateSpeed: EnumItem<"HoverAnimateSpeed">,
  read HttpCachePolicy: EnumItem<"HttpCachePolicy">,
  read HttpCompression: EnumItem<"HttpCompression">,
  read HttpContentType: EnumItem<"HttpContentType">,
  read HttpError: EnumItem<"HttpError">,
  read HttpRequestType: EnumItem<"HttpRequestType">,
  read HumanoidCollisionType: EnumItem<"HumanoidCollisionType">,
  read HumanoidDisplayDistanceType: EnumItem<"HumanoidDisplayDistanceType">,
  read HumanoidHealthDisplayType: EnumItem<"HumanoidHealthDisplayType">,
  read HumanoidRigType: EnumItem<"HumanoidRigType">,
  read HumanoidStateType: EnumItem<"HumanoidStateType">,
  read IKCollisionsMode: EnumItem<"IKCollisionsMode">,
  read IKControlConstraintSupport: EnumItem<"IKControlConstraintSupport">,
  read IKControlType: EnumItem<"IKControlType">,
  read IXPLoadingStatus: EnumItem<"IXPLoadingStatus">,
  read ImageAlphaType: EnumItem<"ImageAlphaType">,
  read ImageCombineType: EnumItem<"ImageCombineType">,
  read InOut: EnumItem<"InOut">,
  read InfoType: EnumItem<"InfoType">,
  read InitialDockState: EnumItem<"InitialDockState">,
  read InputActionType: EnumItem<"InputActionType">,
  read InputBindingType: EnumItem<"InputBindingType">,
  read InputSink: EnumItem<"InputSink">,
  read InputType: EnumItem<"InputType">,
  read InstanceFileSyncStatus: EnumItem<"InstanceFileSyncStatus">,
  read IntermediateMeshGenerationResult: EnumItem<"IntermediateMeshGenerationResult">,
  read InternalVideoUsage: EnumItem<"InternalVideoUsage">,
  read InterpolationThrottlingMode: EnumItem<"InterpolationThrottlingMode">,
  read InviteState: EnumItem<"InviteState">,
  read ItemLineAlignment: EnumItem<"ItemLineAlignment">,
  read JoinSource: EnumItem<"JoinSource">,
  read JointCreationMode: EnumItem<"JointCreationMode">,
  read KeyCode: EnumItem<"KeyCode">,
  read KeyCodeStringFormat: EnumItem<"KeyCodeStringFormat">,
  read KeyInterpolationMode: EnumItem<"KeyInterpolationMode">,
  read KeywordFilterType: EnumItem<"KeywordFilterType">,
  read Language: EnumItem<"Language">,
  read LeftRight: EnumItem<"LeftRight">,
  read LexemeType: EnumItem<"LexemeType">,
  read LightingStyle: EnumItem<"LightingStyle">,
  read Limb: EnumItem<"Limb">,
  read LineJoinMode: EnumItem<"LineJoinMode">,
  read ListDisplayMode: EnumItem<"ListDisplayMode">,
  read ListenerLocation: EnumItem<"ListenerLocation">,
  read ListenerPositionType: EnumItem<"ListenerPositionType">,
  read ListenerType: EnumItem<"ListenerType">,
  read LiveEditingAtomicUpdateResponse: EnumItem<"LiveEditingAtomicUpdateResponse">,
  read LiveEditingBroadcastMessageType: EnumItem<"LiveEditingBroadcastMessageType">,
  read LoadCharacterLayeredClothing: EnumItem<"LoadCharacterLayeredClothing">,
  read LoadDynamicHeads: EnumItem<"LoadDynamicHeads">,
  read LocationType: EnumItem<"LocationType">,
  read LuauTypeCheckMode: EnumItem<"LuauTypeCheckMode">,
  read MakeupType: EnumItem<"MakeupType">,
  read MarketplaceBulkPurchasePromptStatus: EnumItem<"MarketplaceBulkPurchasePromptStatus">,
  read MarketplaceItemPurchaseStatus: EnumItem<"MarketplaceItemPurchaseStatus">,
  read MarketplaceProductType: EnumItem<"MarketplaceProductType">,
  read MarkupKind: EnumItem<"MarkupKind">,
  read MatchmakingType: EnumItem<"MatchmakingType">,
  read Material: EnumItem<"Material">,
  read MaterialPattern: EnumItem<"MaterialPattern">,
  read MembershipType: EnumItem<"MembershipType">,
  read MeshPartDetailLevel: EnumItem<"MeshPartDetailLevel">,
  read MeshPartHeadsAndAccessories: EnumItem<"MeshPartHeadsAndAccessories">,
  read MeshScaleUnit: EnumItem<"MeshScaleUnit">,
  read MeshType: EnumItem<"MeshType">,
  read MessageType: EnumItem<"MessageType">,
  read ModelLevelOfDetail: EnumItem<"ModelLevelOfDetail">,
  read ModelStreamingBehavior: EnumItem<"ModelStreamingBehavior">,
  read ModelStreamingMode: EnumItem<"ModelStreamingMode">,
  read ModerationResultCategory: EnumItem<"ModerationResultCategory">,
  read ModerationResultLabel: EnumItem<"ModerationResultLabel">,
  read ModerationStatus: EnumItem<"ModerationStatus">,
  read ModifierKey: EnumItem<"ModifierKey">,
  read MouseBehavior: EnumItem<"MouseBehavior">,
  read MoveState: EnumItem<"MoveState">,
  read MuteState: EnumItem<"MuteState">,
  read NameOcclusion: EnumItem<"NameOcclusion">,
  read NegateOperationHiddenHistory: EnumItem<"NegateOperationHiddenHistory">,
  read NetworkOwnership: EnumItem<"NetworkOwnership">,
  read NetworkStatus: EnumItem<"NetworkStatus">,
  read NoiseType: EnumItem<"NoiseType">,
  read NormalId: EnumItem<"NormalId">,
  read NotificationButtonType: EnumItem<"NotificationButtonType">,
  read OperationType: EnumItem<"OperationType">,
  read OrientationAlignmentMode: EnumItem<"OrientationAlignmentMode">,
  read OutfitSource: EnumItem<"OutfitSource">,
  read OutfitType: EnumItem<"OutfitType">,
  read OutputLayoutMode: EnumItem<"OutputLayoutMode">,
  read OverrideMouseIconBehavior: EnumItem<"OverrideMouseIconBehavior">,
  read PackagePermission: EnumItem<"PackagePermission">,
  read PartType: EnumItem<"PartType">,
  read ParticleEmitterShape: EnumItem<"ParticleEmitterShape">,
  read ParticleEmitterShapeInOut: EnumItem<"ParticleEmitterShapeInOut">,
  read ParticleEmitterShapeStyle: EnumItem<"ParticleEmitterShapeStyle">,
  read ParticleFlipbookLayout: EnumItem<"ParticleFlipbookLayout">,
  read ParticleFlipbookMode: EnumItem<"ParticleFlipbookMode">,
  read ParticleFlipbookTextureCompatible: EnumItem<"ParticleFlipbookTextureCompatible">,
  read ParticleOrientation: EnumItem<"ParticleOrientation">,
  read PathStatus: EnumItem<"PathStatus">,
  read PathWaypointAction: EnumItem<"PathWaypointAction">,
  read PathfindingUseImprovedSearch: EnumItem<"PathfindingUseImprovedSearch">,
  read PeoplePageLayout: EnumItem<"PeoplePageLayout">,
  read PerformanceOverlayMode: EnumItem<"PerformanceOverlayMode">,
  read PermissionLevelShown: EnumItem<"PermissionLevelShown">,
  read PhysicalConstraintType: EnumItem<"PhysicalConstraintType">,
  read PhysicsSimulationRate: EnumItem<"PhysicsSimulationRate">,
  read PhysicsSteppingMethod: EnumItem<"PhysicsSteppingMethod">,
  read PlaceContentPreference: EnumItem<"PlaceContentPreference">,
  read PlacePublishType: EnumItem<"PlacePublishType">,
  read Platform: EnumItem<"Platform">,
  read PlaybackState: EnumItem<"PlaybackState">,
  read PlayerActions: EnumItem<"PlayerActions">,
  read PlayerCharacterDestroyBehavior: EnumItem<"PlayerCharacterDestroyBehavior">,
  read PlayerChatType: EnumItem<"PlayerChatType">,
  read PlayerDataErrorState: EnumItem<"PlayerDataErrorState">,
  read PlayerDataLoadFailureBehavior: EnumItem<"PlayerDataLoadFailureBehavior">,
  read PlayerExitReason: EnumItem<"PlayerExitReason">,
  read PlayerPlatformActivationStatus: EnumItem<"PlayerPlatformActivationStatus">,
  read PlayerPlatformSpenderStatus: EnumItem<"PlayerPlatformSpenderStatus">,
  read PluginConnectionTargetType: EnumItem<"PluginConnectionTargetType">,
  read PoseEasingDirection: EnumItem<"PoseEasingDirection">,
  read PoseEasingStyle: EnumItem<"PoseEasingStyle">,
  read PositionAlignmentMode: EnumItem<"PositionAlignmentMode">,
  read PredictionMode: EnumItem<"PredictionMode">,
  read PredictionStatus: EnumItem<"PredictionStatus">,
  read PredictiveStreamingMode: EnumItem<"PredictiveStreamingMode">,
  read PreferredInput: EnumItem<"PreferredInput">,
  read PreferredTextSize: EnumItem<"PreferredTextSize">,
  read PrefetchDownloadStatus: EnumItem<"PrefetchDownloadStatus">,
  read PrimalPhysicsSolver: EnumItem<"PrimalPhysicsSolver">,
  read PrimitiveType: EnumItem<"PrimitiveType">,
  read PrivilegeType: EnumItem<"PrivilegeType">,
  read ProductLocationRestriction: EnumItem<"ProductLocationRestriction">,
  read ProductPurchaseChannel: EnumItem<"ProductPurchaseChannel">,
  read ProductPurchaseDecision: EnumItem<"ProductPurchaseDecision">,
  read PromptCreateAssetResult: EnumItem<"PromptCreateAssetResult">,
  read PromptCreateAvatarResult: EnumItem<"PromptCreateAvatarResult">,
  read PromptCreateOutfitResult: EnumItem<"PromptCreateOutfitResult">,
  read PromptExperienceDetailsResult: EnumItem<"PromptExperienceDetailsResult">,
  read PromptLinkSharingResult: EnumItem<"PromptLinkSharingResult">,
  read PromptPublishAssetResult: EnumItem<"PromptPublishAssetResult">,
  read PropertyStatus: EnumItem<"PropertyStatus">,
  read ProximityPromptExclusivity: EnumItem<"ProximityPromptExclusivity">,
  read ProximityPromptInputType: EnumItem<"ProximityPromptInputType">,
  read ProximityPromptStyle: EnumItem<"ProximityPromptStyle">,
  read PurchaseOption: EnumItem<"PurchaseOption">,
  read QualityLevel: EnumItem<"QualityLevel">,
  read R15CollisionType: EnumItem<"R15CollisionType">,
  read RaycastFilterType: EnumItem<"RaycastFilterType">,
  read ReadCapturesFromGalleryResult: EnumItem<"ReadCapturesFromGalleryResult">,
  read ReceiptDecision: EnumItem<"ReceiptDecision">,
  read ReceiptType: EnumItem<"ReceiptType">,
  read RecommendationActionType: EnumItem<"RecommendationActionType">,
  read RecommendationDepartureIntent: EnumItem<"RecommendationDepartureIntent">,
  read RecommendationImpressionType: EnumItem<"RecommendationImpressionType">,
  read RecommendationItemContentType: EnumItem<"RecommendationItemContentType">,
  read RecommendationItemVisibility: EnumItem<"RecommendationItemVisibility">,
  read RecommendationPreferenceTargetType: EnumItem<"RecommendationPreferenceTargetType">,
  read RecommendationPreferenceType: EnumItem<"RecommendationPreferenceType">,
  read RejectCharacterDeletions: EnumItem<"RejectCharacterDeletions">,
  read RenderFidelity: EnumItem<"RenderFidelity">,
  read RenderPriority: EnumItem<"RenderPriority">,
  read RenderingCacheOptimizationMode: EnumItem<"RenderingCacheOptimizationMode">,
  read RenderingTestComparisonMethod: EnumItem<"RenderingTestComparisonMethod">,
  read ReplicateInstanceDestroySetting: EnumItem<"ReplicateInstanceDestroySetting">,
  read ResamplerMode: EnumItem<"ResamplerMode">,
  read ReservedHighlightId: EnumItem<"ReservedHighlightId">,
  read RestPose: EnumItem<"RestPose">,
  read RestPoseModel: EnumItem<"RestPoseModel">,
  read ReturnKeyType: EnumItem<"ReturnKeyType">,
  read ReverbType: EnumItem<"ReverbType">,
  read ReviewableContentState: EnumItem<"ReviewableContentState">,
  read RibbonTool: EnumItem<"RibbonTool">,
  read RigLabel: EnumItem<"RigLabel">,
  read RigScale: EnumItem<"RigScale">,
  read RigType: EnumItem<"RigType">,
  read RollOffMode: EnumItem<"RollOffMode">,
  read RolloutState: EnumItem<"RolloutState">,
  read RotationOrder: EnumItem<"RotationOrder">,
  read RotationType: EnumItem<"RotationType">,
  read RsvpStatus: EnumItem<"RsvpStatus">,
  read RtlTextSupport: EnumItem<"RtlTextSupport">,
  read RunContext: EnumItem<"RunContext">,
  read RunState: EnumItem<"RunState">,
  read RuntimeUndoBehavior: EnumItem<"RuntimeUndoBehavior">,
  read SafeAreaCompatibility: EnumItem<"SafeAreaCompatibility">,
  read SalesTypeFilter: EnumItem<"SalesTypeFilter">,
  read SandboxedInstanceMode: EnumItem<"SandboxedInstanceMode">,
  read SaveAvatarThumbnailCustomizationFailure: EnumItem<"SaveAvatarThumbnailCustomizationFailure">,
  read SaveFilter: EnumItem<"SaveFilter">,
  read SavedQualitySetting: EnumItem<"SavedQualitySetting">,
  read ScaleType: EnumItem<"ScaleType">,
  read ScopeCheckResult: EnumItem<"ScopeCheckResult">,
  read ScreenInsets: EnumItem<"ScreenInsets">,
  read ScreenOrientation: EnumItem<"ScreenOrientation">,
  read ScreenshotCaptureResult: EnumItem<"ScreenshotCaptureResult">,
  read ScriptStoppedReason: EnumItem<"ScriptStoppedReason">,
  read ScriptVariableScope: EnumItem<"ScriptVariableScope">,
  read ScrollBarInset: EnumItem<"ScrollBarInset">,
  read ScrollingDirection: EnumItem<"ScrollingDirection">,
  read SecurityCapability: EnumItem<"SecurityCapability">,
  read SelectionBehavior: EnumItem<"SelectionBehavior">,
  read SelectionRenderMode: EnumItem<"SelectionRenderMode">,
  read SelfViewPosition: EnumItem<"SelfViewPosition">,
  read SensorMode: EnumItem<"SensorMode">,
  read SensorUpdateType: EnumItem<"SensorUpdateType">,
  read ServerLiveEditingMode: EnumItem<"ServerLiveEditingMode">,
  read ServiceVisibility: EnumItem<"ServiceVisibility">,
  read Severity: EnumItem<"Severity">,
  read ShowAdResult: EnumItem<"ShowAdResult">,
  read SignalBehavior: EnumItem<"SignalBehavior">,
  read SimulationMode: EnumItem<"SimulationMode">,
  read SizeConstraint: EnumItem<"SizeConstraint">,
  read SlimTintMode: EnumItem<"SlimTintMode">,
  read SolidPrimitiveType: EnumItem<"SolidPrimitiveType">,
  read SolverConvergenceMetricType: EnumItem<"SolverConvergenceMetricType">,
  read SolverConvergenceVisualizationMode: EnumItem<"SolverConvergenceVisualizationMode">,
  read SortDirection: EnumItem<"SortDirection">,
  read SortOrder: EnumItem<"SortOrder">,
  read SpecialKey: EnumItem<"SpecialKey">,
  read StartCorner: EnumItem<"StartCorner">,
  read StateObjectFieldType: EnumItem<"StateObjectFieldType">,
  read Status: EnumItem<"Status">,
  read StepFrequency: EnumItem<"StepFrequency">,
  read StreamOutBehavior: EnumItem<"StreamOutBehavior">,
  read StreamingIntegrityMode: EnumItem<"StreamingIntegrityMode">,
  read StreamingPauseMode: EnumItem<"StreamingPauseMode">,
  read StrokeSizingMode: EnumItem<"StrokeSizingMode">,
  read StudioAction: EnumItem<"StudioAction">,
  read StudioCaptureBufferStatus: EnumItem<"StudioCaptureBufferStatus">,
  read StudioCaptureScreenshotFormat: EnumItem<"StudioCaptureScreenshotFormat">,
  read StudioCloseMode: EnumItem<"StudioCloseMode">,
  read StudioDataModelType: EnumItem<"StudioDataModelType">,
  read StudioPlaceUpdateFailureReason: EnumItem<"StudioPlaceUpdateFailureReason">,
  read StudioScriptEditorColorCategories: EnumItem<"StudioScriptEditorColorCategories">,
  read StudioScriptEditorColorPresets: EnumItem<"StudioScriptEditorColorPresets">,
  read StudioStyleGuideColor: EnumItem<"StudioStyleGuideColor">,
  read StudioStyleGuideModifier: EnumItem<"StudioStyleGuideModifier">,
  read Style: EnumItem<"Style">,
  read SubscriptionExpirationReason: EnumItem<"SubscriptionExpirationReason">,
  read SubscriptionPaymentStatus: EnumItem<"SubscriptionPaymentStatus">,
  read SubscriptionPeriod: EnumItem<"SubscriptionPeriod">,
  read SubscriptionState: EnumItem<"SubscriptionState">,
  read SurfaceConstraint: EnumItem<"SurfaceConstraint">,
  read SurfaceGuiShape: EnumItem<"SurfaceGuiShape">,
  read SurfaceGuiSizingMode: EnumItem<"SurfaceGuiSizingMode">,
  read SurfaceType: EnumItem<"SurfaceType">,
  read SwipeDirection: EnumItem<"SwipeDirection">,
  read SystemThemeValue: EnumItem<"SystemThemeValue">,
  read TableMajorAxis: EnumItem<"TableMajorAxis">,
  read TagReplicability: EnumItem<"TagReplicability">,
  read TeamCreateErrorState: EnumItem<"TeamCreateErrorState">,
  read Technology: EnumItem<"Technology">,
  read TelemetryBackend: EnumItem<"TelemetryBackend">,
  read TelemetryStandardizedField: EnumItem<"TelemetryStandardizedField">,
  read TeleportMethod: EnumItem<"TeleportMethod">,
  read TeleportResult: EnumItem<"TeleportResult">,
  read TeleportState: EnumItem<"TeleportState">,
  read TeleportType: EnumItem<"TeleportType">,
  read TerrainAcquisitionMethod: EnumItem<"TerrainAcquisitionMethod">,
  read TerrainFace: EnumItem<"TerrainFace">,
  read TerrainLiquidMergeOperation: EnumItem<"TerrainLiquidMergeOperation">,
  read TerrainSolidMergeOperation: EnumItem<"TerrainSolidMergeOperation">,
  read TextChatMessageStatus: EnumItem<"TextChatMessageStatus">,
  read TextDirection: EnumItem<"TextDirection">,
  read TextFilterContext: EnumItem<"TextFilterContext">,
  read TextInputType: EnumItem<"TextInputType">,
  read TextTruncate: EnumItem<"TextTruncate">,
  read TextXAlignment: EnumItem<"TextXAlignment">,
  read TextYAlignment: EnumItem<"TextYAlignment">,
  read TextureMode: EnumItem<"TextureMode">,
  read TextureQueryType: EnumItem<"TextureQueryType">,
  read ThreadPoolConfig: EnumItem<"ThreadPoolConfig">,
  read ThrottlingPriority: EnumItem<"ThrottlingPriority">,
  read ThumbnailSize: EnumItem<"ThumbnailSize">,
  read ThumbnailType: EnumItem<"ThumbnailType">,
  read TickCountSampleMethod: EnumItem<"TickCountSampleMethod">,
  read TonemapperPreset: EnumItem<"TonemapperPreset">,
  read TopBottom: EnumItem<"TopBottom">,
  read TouchCameraMovementMode: EnumItem<"TouchCameraMovementMode">,
  read TouchMovementMode: EnumItem<"TouchMovementMode">,
  read TrackerError: EnumItem<"TrackerError">,
  read TrackerExtrapolationFlagMode: EnumItem<"TrackerExtrapolationFlagMode">,
  read TrackerFaceTrackingStatus: EnumItem<"TrackerFaceTrackingStatus">,
  read TrackerLodFlagMode: EnumItem<"TrackerLodFlagMode">,
  read TrackerLodValueMode: EnumItem<"TrackerLodValueMode">,
  read TrackerMode: EnumItem<"TrackerMode">,
  read TrackerPromptEvent: EnumItem<"TrackerPromptEvent">,
  read TrackerType: EnumItem<"TrackerType">,
  read TriStateBoolean: EnumItem<"TriStateBoolean">,
  read TweenStatus: EnumItem<"TweenStatus">,
  read UICaptureMode: EnumItem<"UICaptureMode">,
  read UIDragDetectorBoundingBehavior: EnumItem<"UIDragDetectorBoundingBehavior">,
  read UIDragDetectorDragRelativity: EnumItem<"UIDragDetectorDragRelativity">,
  read UIDragDetectorDragSpace: EnumItem<"UIDragDetectorDragSpace">,
  read UIDragDetectorDragStyle: EnumItem<"UIDragDetectorDragStyle">,
  read UIDragDetectorResponseStyle: EnumItem<"UIDragDetectorResponseStyle">,
  read UIDragSpeedAxisMapping: EnumItem<"UIDragSpeedAxisMapping">,
  read UIFlexAlignment: EnumItem<"UIFlexAlignment">,
  read UIFlexMode: EnumItem<"UIFlexMode">,
  read UITheme: EnumItem<"UITheme">,
  read UiMessageType: EnumItem<"UiMessageType">,
  read UpdateState: EnumItem<"UpdateState">,
  read UploadCaptureResult: EnumItem<"UploadCaptureResult">,
  read UsageContext: EnumItem<"UsageContext">,
  read UserCFrame: EnumItem<"UserCFrame">,
  read UserInputState: EnumItem<"UserInputState">,
  read UserInputType: EnumItem<"UserInputType">,
  read VRComfortSetting: EnumItem<"VRComfortSetting">,
  read VRControllerModelMode: EnumItem<"VRControllerModelMode">,
  read VRDeviceType: EnumItem<"VRDeviceType">,
  read VRLaserPointerMode: EnumItem<"VRLaserPointerMode">,
  read VRSafetyBubbleMode: EnumItem<"VRSafetyBubbleMode">,
  read VRScaling: EnumItem<"VRScaling">,
  read VRSessionState: EnumItem<"VRSessionState">,
  read VRTouchpad: EnumItem<"VRTouchpad">,
  read VRTouchpadMode: EnumItem<"VRTouchpadMode">,
  read VelocityConstraintMode: EnumItem<"VelocityConstraintMode">,
  read VerticalAlignment: EnumItem<"VerticalAlignment">,
  read VerticalScrollBarPosition: EnumItem<"VerticalScrollBarPosition">,
  read VibrationMotor: EnumItem<"VibrationMotor">,
  read VideoCaptureResult: EnumItem<"VideoCaptureResult">,
  read VideoCaptureStartedResult: EnumItem<"VideoCaptureStartedResult">,
  read VideoDeviceCaptureQuality: EnumItem<"VideoDeviceCaptureQuality">,
  read VideoError: EnumItem<"VideoError">,
  read VideoSampleSize: EnumItem<"VideoSampleSize">,
  read ViewMode: EnumItem<"ViewMode">,
  read VirtualCursorMode: EnumItem<"VirtualCursorMode">,
  read VirtualInputMode: EnumItem<"VirtualInputMode">,
  read VoiceChatDistanceAttenuationType: EnumItem<"VoiceChatDistanceAttenuationType">,
  read VoiceChatState: EnumItem<"VoiceChatState">,
  read VoiceClientLeaveReasons: EnumItem<"VoiceClientLeaveReasons">,
  read VoiceControlPath: EnumItem<"VoiceControlPath">,
  read VoiceRccReconnectReason: EnumItem<"VoiceRccReconnectReason">,
  read VolumetricAudio: EnumItem<"VolumetricAudio">,
  read WaterDirection: EnumItem<"WaterDirection">,
  read WaterForce: EnumItem<"WaterForce">,
  read WebSocketState: EnumItem<"WebSocketState">,
  read WebStreamClientState: EnumItem<"WebStreamClientState">,
  read WebStreamClientType: EnumItem<"WebStreamClientType">,
  read WeldConstraintPreserve: EnumItem<"WeldConstraintPreserve">,
  read WhenUserFirstPlayed: EnumItem<"WhenUserFirstPlayed">,
  read WhisperChatPrivacyMode: EnumItem<"WhisperChatPrivacyMode">,
  read WrapLayerAutoSkin: EnumItem<"WrapLayerAutoSkin">,
  read WrapLayerDebugMode: EnumItem<"WrapLayerDebugMode">,
  read WrapTargetDebugMode: EnumItem<"WrapTargetDebugMode">,
  read ZIndexBehavior: EnumItem<"ZIndexBehavior">,
}
const water: index<Enum, "Material"> = Enum.Material.Water
print(water)
print(water.Name)
print(water.Value)
