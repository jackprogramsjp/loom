-- This is an automatically generated file and it should not be edited manually.
-- Generated Roblox enums
type EnumItem = {
  read Name: string,
  read Value: number,
  read EnumType: never,
}
type AccessModifierType = {
  read Allow: EnumItem,
  read Deny: EnumItem,
}
type AccessoryType = {
  read Unknown: EnumItem,
  read Hat: EnumItem,
  read Hair: EnumItem,
  read Face: EnumItem,
  read Neck: EnumItem,
  read Shoulder: EnumItem,
  read Front: EnumItem,
  read Back: EnumItem,
  read Waist: EnumItem,
  read TShirt: EnumItem,
  read Shirt: EnumItem,
  read Pants: EnumItem,
  read Jacket: EnumItem,
  read Sweater: EnumItem,
  read Shorts: EnumItem,
  read LeftShoe: EnumItem,
  read RightShoe: EnumItem,
  read DressSkirt: EnumItem,
  read Eyebrow: EnumItem,
  read Eyelash: EnumItem,
}
type ActionOnAutoResumeSync = {
  read DontResume: EnumItem,
  read KeepStudio: EnumItem,
  read KeepLocal: EnumItem,
}
type ActionOnStopSync = {
  read AlwaysAsk: EnumItem,
  read KeepLocalFiles: EnumItem,
  read DeleteLocalFiles: EnumItem,
}
type ActionType = {
  read Nothing: EnumItem,
  read Pause: EnumItem,
  read Lose: EnumItem,
  read Draw: EnumItem,
  read Win: EnumItem,
}
type ActivePayerStatus = {
  read Unknown: EnumItem,
  read Never: EnumItem,
  read Lapsed: EnumItem,
  read Casual50Percent: EnumItem,
  read Intermediate35Percent: EnumItem,
  read Top15Percent: EnumItem,
}
type ActuatorRelativeTo = {
  read Attachment0: EnumItem,
  read Attachment1: EnumItem,
  read World: EnumItem,
}
type ActuatorType = {
  read None: EnumItem,
  read Motor: EnumItem,
  read Servo: EnumItem,
}
type AdAvailabilityResult = {
  read IsAvailable: EnumItem,
  read DeviceIneligible: EnumItem,
  read ExperienceIneligible: EnumItem,
  read InternalError: EnumItem,
  read NoFill: EnumItem,
  read PlayerIneligible: EnumItem,
  read PublisherIneligible: EnumItem,
}
type AdEventType = {
  read VideoLoaded: EnumItem,
  read VideoRemoved: EnumItem,
  read UserCompletedVideo: EnumItem,
  read RewardedAdLoaded: EnumItem,
  read RewardedAdGrant: EnumItem,
  read RewardedAdUnloaded: EnumItem,
}
type AdFormat = {
  read RewardedVideo: EnumItem,
}
type AdShape = {
  read HorizontalRectangle: EnumItem,
}
type AdTeleportMethod = {
  read Undefined: EnumItem,
  read PortalForward: EnumItem,
  read InGameMenuBackButton: EnumItem,
  read UIBackButton: EnumItem,
}
type AdUIEventType = {
  read AdLabelClicked: EnumItem,
  read VolumeButtonClicked: EnumItem,
  read FullscreenButtonClicked: EnumItem,
  read PlayButtonClicked: EnumItem,
  read PauseButtonClicked: EnumItem,
  read CloseButtonClicked: EnumItem,
  read WhyThisAdClicked: EnumItem,
  read PlayEventTriggered: EnumItem,
  read PauseEventTriggered: EnumItem,
}
type AdUIType = {
  read None: EnumItem,
  read Image: EnumItem,
  read Video: EnumItem,
}
type AdUnitStatus = {
  read Inactive: EnumItem,
  read Active: EnumItem,
}
type AdornCullingMode = {
  read Automatic: EnumItem,
  read Never: EnumItem,
}
type AdornShading = {
  read Default: EnumItem,
  read Shaded: EnumItem,
  read XRay: EnumItem,
  read XRayShaded: EnumItem,
  read AlwaysOnTop: EnumItem,
}
type AgeCheckStatus = {
  read Unchecked: EnumItem,
  read Checked: EnumItem,
}
type AlignType = {
  read Parallel: EnumItem,
  read Perpendicular: EnumItem,
  read PrimaryAxisParallel: EnumItem,
  read PrimaryAxisPerpendicular: EnumItem,
  read PrimaryAxisLookAt: EnumItem,
  read AllAxes: EnumItem,
}
type AlphaMode = {
  read Overlay: EnumItem,
  read Transparency: EnumItem,
  read TintMask: EnumItem,
  read Opaque: EnumItem,
}
type AnalyticsCustomFieldKeys = {
  read CustomField01: EnumItem,
  read CustomField02: EnumItem,
  read CustomField03: EnumItem,
}
type AnalyticsEconomyAction = {
  read Default: EnumItem,
  read Acquire: EnumItem,
  read Spend: EnumItem,
}
type AnalyticsEconomyFlowType = {
  read Sink: EnumItem,
  read Source: EnumItem,
}
type AnalyticsEconomyTransactionType = {
  read IAP: EnumItem,
  read Shop: EnumItem,
  read Gameplay: EnumItem,
  read ContextualPurchase: EnumItem,
  read TimedReward: EnumItem,
  read Onboarding: EnumItem,
}
type AnalyticsLogLevel = {
  read Trace: EnumItem,
  read Debug: EnumItem,
  read Information: EnumItem,
  read Warning: EnumItem,
  read Error: EnumItem,
  read Fatal: EnumItem,
}
type AnalyticsProgressionStatus = {
  read Default: EnumItem,
  read Begin: EnumItem,
  read Complete: EnumItem,
  read Abandon: EnumItem,
  read Fail: EnumItem,
}
type AnalyticsProgressionType = {
  read Custom: EnumItem,
  read Start: EnumItem,
  read Fail: EnumItem,
  read Complete: EnumItem,
}
type AnimationClipFromVideoStatus = {
  read Initializing: EnumItem,
  read Pending: EnumItem,
  read Processing: EnumItem,
  read ErrorGeneric: EnumItem,
  read Success: EnumItem,
  read ErrorVideoTooLong: EnumItem,
  read ErrorNoPersonDetected: EnumItem,
  read ErrorVideoUnstable: EnumItem,
  read Timeout: EnumItem,
  read Cancelled: EnumItem,
  read ErrorMultiplePeople: EnumItem,
  read ErrorUploadingVideo: EnumItem,
}
type AnimationNodeBlend2DInputMode = {
  read Cartesian: EnumItem,
  read Polar: EnumItem,
}
type AnimationNodeInterruptible = {
  read Always: EnumItem,
  read Finished: EnumItem,
  read Trigger: EnumItem,
}
type AnimationNodePhaseSync = {
  read Synced: EnumItem,
  read Unsynced: EnumItem,
}
type AnimationNodePlayMode = {
  read Loop: EnumItem,
  read PingPong: EnumItem,
  read OnceAndHold: EnumItem,
  read OnceAndReset: EnumItem,
}
type AnimationNodeTransitionType = {
  read CrossFade: EnumItem,
  read InertialBlend: EnumItem,
  read DeadBlend: EnumItem,
}
type AnimationNodeType = {
  read InvalidNode: EnumItem,
  read AddNode: EnumItem,
  read OverNode: EnumItem,
  read Blend1DNode: EnumItem,
  read Blend2DNode: EnumItem,
  read ClipNode: EnumItem,
  read GraphOutput: EnumItem,
  read MaskNode: EnumItem,
  read PrioritySelectNode: EnumItem,
  read RandomSequenceNode: EnumItem,
  read SelectNode: EnumItem,
  read SequenceNode: EnumItem,
  read SpeedNode: EnumItem,
  read SubtractNode: EnumItem,
}
type AnimationNodeWaitFor = {
  read Finished: EnumItem,
  read Trigger: EnumItem,
}
type AnimationPriority = {
  read Idle: EnumItem,
  read Movement: EnumItem,
  read Action: EnumItem,
  read Action2: EnumItem,
  read Action3: EnumItem,
  read Action4: EnumItem,
  read Core: EnumItem,
}
type AnimatorRetargetingMode = {
  read Default: EnumItem,
  read Disabled: EnumItem,
  read Enabled: EnumItem,
}
type AnnotationChannelContentPreference = {
  read None: EnumItem,
  read All: EnumItem,
  read Unknown: EnumItem,
}
type AnnotationEditingMode = {
  read None: EnumItem,
  read PlacingNew: EnumItem,
  read WritingNew: EnumItem,
}
type AnnotationPlaceContentPreference = {
  read None: EnumItem,
  read All: EnumItem,
  read MentionsAndReplies: EnumItem,
  read Unknown: EnumItem,
}
type AnnotationRequestStatus = {
  read Success: EnumItem,
  read Loading: EnumItem,
  read ErrorInternalFailure: EnumItem,
  read ErrorNotFound: EnumItem,
  read ErrorModerated: EnumItem,
}
type AnnotationRequestType = {
  read Unknown: EnumItem,
  read Create: EnumItem,
  read Resolve: EnumItem,
  read Delete: EnumItem,
  read Edit: EnumItem,
}
type AntiAliasing = {
  read Disabled: EnumItem,
  read Enabled: EnumItem,
}
type AppLifecycleManagerState = {
  read Detached: EnumItem,
  read Active: EnumItem,
  read Inactive: EnumItem,
  read Hidden: EnumItem,
}
type AppShellActionType = {
  read None: EnumItem,
  read OpenApp: EnumItem,
  read TapChatTab: EnumItem,
  read TapConversationEntry: EnumItem,
  read TapAvatarTab: EnumItem,
  read ReadConversation: EnumItem,
  read TapGamePageTab: EnumItem,
  read TapHomePageTab: EnumItem,
  read GamePageLoaded: EnumItem,
  read HomePageLoaded: EnumItem,
  read AvatarEditorPageLoaded: EnumItem,
  read HomePageInteractive: EnumItem,
}
type AppShellFeature = {
  read None: EnumItem,
  read Chat: EnumItem,
  read AvatarEditor: EnumItem,
  read GamePage: EnumItem,
  read HomePage: EnumItem,
  read More: EnumItem,
  read Landing: EnumItem,
  read WatchPage: EnumItem,
}
type AppUpdateStatus = {
  read Unknown: EnumItem,
  read NotSupported: EnumItem,
  read Failed: EnumItem,
  read NotAvailable: EnumItem,
  read Available: EnumItem,
  read AvailableBoundChannel: EnumItem,
  read AvailableBetaProgram: EnumItem,
}
type ApplyStrokeMode = {
  read Contextual: EnumItem,
  read Border: EnumItem,
}
type AspectType = {
  read FitWithinMaxSize: EnumItem,
  read ScaleWithParentSize: EnumItem,
}
type AssetCreatorType = {
  read User: EnumItem,
  read Group: EnumItem,
}
type AssetFetchStatus = {
  read Success: EnumItem,
  read Failure: EnumItem,
  read None: EnumItem,
  read Loading: EnumItem,
  read TimedOut: EnumItem,
}
type AssetRepresentation = {
  read FullLength: EnumItem,
  read ShortPreview: EnumItem,
}
type AssetType = {
  read Image: EnumItem,
  read TShirt: EnumItem,
  read Audio: EnumItem,
  read Mesh: EnumItem,
  read Lua: EnumItem,
  read Hat: EnumItem,
  read Place: EnumItem,
  read Model: EnumItem,
  read Shirt: EnumItem,
  read Pants: EnumItem,
  read Decal: EnumItem,
  read Head: EnumItem,
  read Face: EnumItem,
  read Gear: EnumItem,
  read Badge: EnumItem,
  read Animation: EnumItem,
  read Torso: EnumItem,
  read RightArm: EnumItem,
  read LeftArm: EnumItem,
  read LeftLeg: EnumItem,
  read RightLeg: EnumItem,
  read Package: EnumItem,
  read GamePass: EnumItem,
  read Plugin: EnumItem,
  read MeshPart: EnumItem,
  read HairAccessory: EnumItem,
  read FaceAccessory: EnumItem,
  read NeckAccessory: EnumItem,
  read ShoulderAccessory: EnumItem,
  read FrontAccessory: EnumItem,
  read BackAccessory: EnumItem,
  read WaistAccessory: EnumItem,
  read ClimbAnimation: EnumItem,
  read DeathAnimation: EnumItem,
  read FallAnimation: EnumItem,
  read IdleAnimation: EnumItem,
  read JumpAnimation: EnumItem,
  read RunAnimation: EnumItem,
  read SwimAnimation: EnumItem,
  read WalkAnimation: EnumItem,
  read PoseAnimation: EnumItem,
  read EarAccessory: EnumItem,
  read EyeAccessory: EnumItem,
  read EmoteAnimation: EnumItem,
  read Video: EnumItem,
  read TShirtAccessory: EnumItem,
  read ShirtAccessory: EnumItem,
  read PantsAccessory: EnumItem,
  read JacketAccessory: EnumItem,
  read SweaterAccessory: EnumItem,
  read ShortsAccessory: EnumItem,
  read LeftShoeAccessory: EnumItem,
  read RightShoeAccessory: EnumItem,
  read DressSkirtAccessory: EnumItem,
  read FontFamily: EnumItem,
  read EyebrowAccessory: EnumItem,
  read EyelashAccessory: EnumItem,
  read MoodAnimation: EnumItem,
  read DynamicHead: EnumItem,
  read FaceMakeup: EnumItem,
  read LipMakeup: EnumItem,
  read EyeMakeup: EnumItem,
  read VoxelFragment: EnumItem,
  read AvatarBackground: EnumItem,
  read TextDocument: EnumItem,
}
type AssetTypeVerification = {
  read Default: EnumItem,
  read ClientOnly: EnumItem,
  read Always: EnumItem,
}
type AudioApiRollout = {
  read Disabled: EnumItem,
  read Automatic: EnumItem,
  read Enabled: EnumItem,
}
type AudioCaptureMode = {}
type AudioChannelLayout = {
  read Mono: EnumItem,
  read Stereo: EnumItem,
  read Quad: EnumItem,
  read Surround_5: EnumItem,
  read Surround_5_1: EnumItem,
  read Surround_7_1: EnumItem,
  read Surround_7_1_4: EnumItem,
}
type AudioFilterType = {
  read Peak: EnumItem,
  read LowShelf: EnumItem,
  read HighShelf: EnumItem,
  read Lowpass12dB: EnumItem,
  read Lowpass24dB: EnumItem,
  read Lowpass48dB: EnumItem,
  read Highpass12dB: EnumItem,
  read Highpass24dB: EnumItem,
  read Highpass48dB: EnumItem,
  read Bandpass: EnumItem,
  read Notch: EnumItem,
  read Lowpass6dB: EnumItem,
}
type AudioSimulationFidelity = {
  read None: EnumItem,
  read Automatic: EnumItem,
}
type AudioSubType = {
  read Music: EnumItem,
  read SoundEffect: EnumItem,
}
type AudioWindowSize = {
  read Small: EnumItem,
  read Medium: EnumItem,
  read Large: EnumItem,
}
type AuthorityMode = {
  read Server: EnumItem,
  read Automatic: EnumItem,
}
type AutoIndentRule = {
  read Off: EnumItem,
  read Absolute: EnumItem,
  read Relative: EnumItem,
}
type AutomaticSize = {
  read None: EnumItem,
  read X: EnumItem,
  read Y: EnumItem,
  read XY: EnumItem,
}
type AvatarAssetType = {
  read TShirt: EnumItem,
  read Hat: EnumItem,
  read Shirt: EnumItem,
  read Pants: EnumItem,
  read Head: EnumItem,
  read Face: EnumItem,
  read Gear: EnumItem,
  read Torso: EnumItem,
  read RightArm: EnumItem,
  read LeftArm: EnumItem,
  read LeftLeg: EnumItem,
  read RightLeg: EnumItem,
  read HairAccessory: EnumItem,
  read FaceAccessory: EnumItem,
  read NeckAccessory: EnumItem,
  read ShoulderAccessory: EnumItem,
  read FrontAccessory: EnumItem,
  read BackAccessory: EnumItem,
  read WaistAccessory: EnumItem,
  read ClimbAnimation: EnumItem,
  read FallAnimation: EnumItem,
  read IdleAnimation: EnumItem,
  read JumpAnimation: EnumItem,
  read RunAnimation: EnumItem,
  read SwimAnimation: EnumItem,
  read WalkAnimation: EnumItem,
  read EmoteAnimation: EnumItem,
  read TShirtAccessory: EnumItem,
  read ShirtAccessory: EnumItem,
  read PantsAccessory: EnumItem,
  read JacketAccessory: EnumItem,
  read SweaterAccessory: EnumItem,
  read ShortsAccessory: EnumItem,
  read LeftShoeAccessory: EnumItem,
  read RightShoeAccessory: EnumItem,
  read DressSkirtAccessory: EnumItem,
  read EyebrowAccessory: EnumItem,
  read EyelashAccessory: EnumItem,
  read MoodAnimation: EnumItem,
  read DynamicHead: EnumItem,
  read FaceMakeup: EnumItem,
  read LipMakeup: EnumItem,
  read EyeMakeup: EnumItem,
  read AvatarBackground: EnumItem,
}
type AvatarChatServiceFeature = {
  read None: EnumItem,
  read UniverseAudio: EnumItem,
  read UniverseVideo: EnumItem,
  read PlaceAudio: EnumItem,
  read PlaceVideo: EnumItem,
  read UserAudioEligible: EnumItem,
  read UserAudio: EnumItem,
  read UserVideoEligible: EnumItem,
  read UserVideo: EnumItem,
  read UserBanned: EnumItem,
  read UserVerifiedForVoice: EnumItem,
}
type AvatarContextMenuOption = {
  read Friend: EnumItem,
  read Chat: EnumItem,
  read Emote: EnumItem,
  read InspectMenu: EnumItem,
}
type AvatarGenerationError = {
  read None: EnumItem,
  read Unknown: EnumItem,
  read DownloadFailed: EnumItem,
  read Canceled: EnumItem,
  read Offensive: EnumItem,
  read Timeout: EnumItem,
  read JobNotFound: EnumItem,
}
type AvatarItemType = {
  read Asset: EnumItem,
  read Bundle: EnumItem,
}
type AvatarPromptResult = {
  read Success: EnumItem,
  read PermissionDenied: EnumItem,
  read Failed: EnumItem,
}
type AvatarSettingsAccessoryLimitMethod = {
  read Scale: EnumItem,
  read Remove: EnumItem,
  read PreviewScale: EnumItem,
  read PreviewRemove: EnumItem,
}
type AvatarSettingsAccessoryMode = {
  read PlayerChoice: EnumItem,
  read CustomLimit: EnumItem,
}
type AvatarSettingsAnimationClipsMode = {
  read PlayerChoice: EnumItem,
  read CustomClips: EnumItem,
}
type AvatarSettingsAnimationPacksMode = {
  read PlayerChoice: EnumItem,
  read StandardR15: EnumItem,
  read StandardR6: EnumItem,
}
type AvatarSettingsAppearanceMode = {
  read PlayerChoice: EnumItem,
  read CustomParts: EnumItem,
  read CustomBody: EnumItem,
}
type AvatarSettingsBuildMode = {
  read PlayerChoice: EnumItem,
  read CustomBuild: EnumItem,
}
type AvatarSettingsCharacterControllerMode = {
  read LegacyHumanoid: EnumItem,
  read LuaCharacterController: EnumItem,
}
type AvatarSettingsClothingMode = {
  read PlayerChoice: EnumItem,
  read CustomLimit: EnumItem,
}
type AvatarSettingsCollisionMode = {
  read Default: EnumItem,
  read SingleCollider: EnumItem,
  read Legacy: EnumItem,
}
type AvatarSettingsCustomAccessoryMode = {
  read PlayerChoice: EnumItem,
  read CustomAccessories: EnumItem,
}
type AvatarSettingsCustomBodyType = {
  read AvatarReference: EnumItem,
  read BundleId: EnumItem,
}
type AvatarSettingsCustomClothingMode = {
  read PlayerChoice: EnumItem,
  read CustomClothing: EnumItem,
}
type AvatarSettingsHitAndTouchDetectionMode = {
  read UseParts: EnumItem,
  read UseCollider: EnumItem,
}
type AvatarSettingsJumpMode = {
  read JumpHeight: EnumItem,
  read JumpPower: EnumItem,
}
type AvatarSettingsLegacyCollisionMode = {
  read R6Colliders: EnumItem,
  read InnerBoxColliders: EnumItem,
}
type AvatarSettingsScaleMode = {
  read PlayerChoice: EnumItem,
  read CustomScale: EnumItem,
}
type AvatarThumbnailCustomizationType = {
  read Closeup: EnumItem,
  read FullBody: EnumItem,
}
type AvatarUnificationMode = {
  read Default: EnumItem,
  read Disabled: EnumItem,
  read Enabled: EnumItem,
}
type Axis = {
  read X: EnumItem,
  read Y: EnumItem,
  read Z: EnumItem,
}
type BenefitType = {
  read DeveloperProduct: EnumItem,
  read AvatarAsset: EnumItem,
  read AvatarBundle: EnumItem,
}
type BinType = {
  read Script: EnumItem,
  read GameTool: EnumItem,
  read Grab: EnumItem,
  read Clone: EnumItem,
  read Hammer: EnumItem,
}
type BodyPart = {
  read Head: EnumItem,
  read Torso: EnumItem,
  read LeftArm: EnumItem,
  read RightArm: EnumItem,
  read LeftLeg: EnumItem,
  read RightLeg: EnumItem,
}
type BodyPartR15 = {
  read Head: EnumItem,
  read UpperTorso: EnumItem,
  read LowerTorso: EnumItem,
  read LeftFoot: EnumItem,
  read LeftLowerLeg: EnumItem,
  read LeftUpperLeg: EnumItem,
  read RightFoot: EnumItem,
  read RightLowerLeg: EnumItem,
  read RightUpperLeg: EnumItem,
  read LeftHand: EnumItem,
  read LeftLowerArm: EnumItem,
  read LeftUpperArm: EnumItem,
  read RightHand: EnumItem,
  read RightLowerArm: EnumItem,
  read RightUpperArm: EnumItem,
  read RootPart: EnumItem,
  read Unknown: EnumItem,
}
type BorderMode = {
  read Outline: EnumItem,
  read Middle: EnumItem,
  read Inset: EnumItem,
}
type BorderStrokePosition = {
  read Outer: EnumItem,
  read Center: EnumItem,
  read Inner: EnumItem,
}
type BreakReason = {
  read Other: EnumItem,
  read Error: EnumItem,
  read SpecialBreakpoint: EnumItem,
  read UserBreakpoint: EnumItem,
}
type BreakpointRemoveReason = {
  read Requested: EnumItem,
  read ScriptChanged: EnumItem,
  read ScriptRemoved: EnumItem,
}
type BulkMoveMode = {
  read FireAllEvents: EnumItem,
  read FireCFrameChanged: EnumItem,
}
type BundleType = {
  read BodyParts: EnumItem,
  read Animations: EnumItem,
  read Shoes: EnumItem,
  read DynamicHead: EnumItem,
  read DynamicHeadAvatar: EnumItem,
}
type Button = {
  read Dismount: EnumItem,
  read Jump: EnumItem,
}
type ButtonStyle = {
  read Custom: EnumItem,
  read RobloxButtonDefault: EnumItem,
  read RobloxButton: EnumItem,
  read RobloxRoundButton: EnumItem,
  read RobloxRoundDefaultButton: EnumItem,
  read RobloxRoundDropdownButton: EnumItem,
}
type CageType = {
  read Inner: EnumItem,
  read Outer: EnumItem,
}
type CameraMode = {
  read Classic: EnumItem,
  read LockFirstPerson: EnumItem,
}
type CameraNavigationModel = {
  read Roblox: EnumItem,
  read IndustryCompatible: EnumItem,
}
type CameraPanMode = {
  read Classic: EnumItem,
  read EdgeBump: EnumItem,
}
type CameraSpeedAdjustBinding = {
  read None: EnumItem,
  read RmbScroll: EnumItem,
  read AltScroll: EnumItem,
}
type CameraType = {
  read Fixed: EnumItem,
  read Attach: EnumItem,
  read Watch: EnumItem,
  read Track: EnumItem,
  read Follow: EnumItem,
  read Custom: EnumItem,
  read Scriptable: EnumItem,
  read Orbital: EnumItem,
}
type CanCollaborateError = {
  read Invalid: EnumItem,
  read None: EnumItem,
  read NotAgeVerified: EnumItem,
  read OutsideAgeBucket: EnumItem,
  read TooManyCollaborators: EnumItem,
  read PCBlock: EnumItem,
  read NotFound: EnumItem,
  read OutsideOwnerAgeBucket: EnumItem,
  read NotAuthorized: EnumItem,
  read OutsideAgeBucketTcPc: EnumItem,
  read OtherCollaboratorSettingsPreventTrust: EnumItem,
  read AgeVerificationCountryBlocked: EnumItem,
  read OtherUserCannotCollaborate: EnumItem,
}
type CaptureGalleryPermission = {
  read ReadAndUpload: EnumItem,
}
type CaptureType = {
  read Screenshot: EnumItem,
  read Video: EnumItem,
}
type CatalogCategoryFilter = {
  read None: EnumItem,
  read Featured: EnumItem,
  read Collectibles: EnumItem,
  read CommunityCreations: EnumItem,
  read Premium: EnumItem,
  read Recommended: EnumItem,
}
type CatalogSortAggregation = {
  read Past12Hours: EnumItem,
  read PastDay: EnumItem,
  read Past3Days: EnumItem,
  read PastWeek: EnumItem,
  read PastMonth: EnumItem,
  read AllTime: EnumItem,
}
type CatalogSortType = {
  read Relevance: EnumItem,
  read PriceHighToLow: EnumItem,
  read PriceLowToHigh: EnumItem,
  read MostFavorited: EnumItem,
  read RecentlyCreated: EnumItem,
  read Bestselling: EnumItem,
}
type CellBlock = {
  read Solid: EnumItem,
  read VerticalWedge: EnumItem,
  read CornerWedge: EnumItem,
  read InverseCornerWedge: EnumItem,
  read HorizontalWedge: EnumItem,
}
type CellMaterial = {
  read Empty: EnumItem,
  read Grass: EnumItem,
  read Sand: EnumItem,
  read Brick: EnumItem,
  read Granite: EnumItem,
  read Asphalt: EnumItem,
  read Iron: EnumItem,
  read Aluminum: EnumItem,
  read Gold: EnumItem,
  read WoodPlank: EnumItem,
  read WoodLog: EnumItem,
  read Gravel: EnumItem,
  read CinderBlock: EnumItem,
  read MossyStone: EnumItem,
  read Cement: EnumItem,
  read RedPlastic: EnumItem,
  read BluePlastic: EnumItem,
  read Water: EnumItem,
}
type CellOrientation = {
  read NegZ: EnumItem,
  read X: EnumItem,
  read Z: EnumItem,
  read NegX: EnumItem,
}
type CenterDialogType = {
  read UnsolicitedDialog: EnumItem,
  read PlayerInitiatedDialog: EnumItem,
  read ModalDialog: EnumItem,
  read QuitDialog: EnumItem,
}
type CharacterControlMode = {
  read Default: EnumItem,
  read Legacy: EnumItem,
  read NoCharacterController: EnumItem,
  read LuaCharacterController: EnumItem,
}
type ChatCallbackType = {
  read OnCreatingChatWindow: EnumItem,
  read OnClientSendingMessage: EnumItem,
  read OnClientFormattingMessage: EnumItem,
  read OnServerReceivingMessage: EnumItem,
}
type ChatColor = {
  read Blue: EnumItem,
  read Green: EnumItem,
  read Red: EnumItem,
  read White: EnumItem,
}
type ChatMode = {
  read Menu: EnumItem,
  read TextAndMenu: EnumItem,
}
type ChatPrivacyMode = {
  read AllUsers: EnumItem,
  read NoOne: EnumItem,
  read Friends: EnumItem,
}
type ChatRestrictionStatus = {
  read Unknown: EnumItem,
  read NotRestricted: EnumItem,
  read Restricted: EnumItem,
}
type ChatStyle = {
  read Classic: EnumItem,
  read Bubble: EnumItem,
  read ClassicAndBubble: EnumItem,
}
type ChatVersion = {
  read LegacyChatService: EnumItem,
  read TextChatService: EnumItem,
}
type ClientAnimatorThrottlingMode = {
  read Default: EnumItem,
  read Disabled: EnumItem,
  read Enabled: EnumItem,
}
type CloseReason = {
  read Unknown: EnumItem,
  read RobloxMaintenance: EnumItem,
  read DeveloperShutdown: EnumItem,
  read DeveloperUpdate: EnumItem,
  read ServerEmpty: EnumItem,
  read OutOfMemory: EnumItem,
  read Moderation: EnumItem,
}
type CollaboratorStatus = {
  read None: EnumItem,
  read Editing3D: EnumItem,
  read Scripting: EnumItem,
  read PrivateScripting: EnumItem,
}
type CollisionFidelity = {
  read Default: EnumItem,
  read Hull: EnumItem,
  read Box: EnumItem,
  read PreciseConvexDecomposition: EnumItem,
  read Scalable: EnumItem,
}
type CommandPermission = {
  read Plugin: EnumItem,
  read LocalUser: EnumItem,
}
type CompileTarget = {
  read Client: EnumItem,
  read CoreScript: EnumItem,
  read Studio: EnumItem,
  read CoreScriptRaw: EnumItem,
}
type CompletionAcceptanceBehavior = {
  read Insert: EnumItem,
  read Replace: EnumItem,
  read ReplaceOnEnterInsertOnTab: EnumItem,
  read InsertOnEnterReplaceOnTab: EnumItem,
}
type CompletionItemKind = {
  read Text: EnumItem,
  read Method: EnumItem,
  read Function: EnumItem,
  read Constructor: EnumItem,
  read Field: EnumItem,
  read Variable: EnumItem,
  read Class: EnumItem,
  read Interface: EnumItem,
  read Module: EnumItem,
  read Property: EnumItem,
  read Unit: EnumItem,
  read Value: EnumItem,
  read Enum: EnumItem,
  read Keyword: EnumItem,
  read Snippet: EnumItem,
  read Color: EnumItem,
  read File: EnumItem,
  read Reference: EnumItem,
  read Folder: EnumItem,
  read EnumMember: EnumItem,
  read Constant: EnumItem,
  read Struct: EnumItem,
  read Event: EnumItem,
  read Operator: EnumItem,
  read TypeParameter: EnumItem,
}
type CompletionItemTag = {
  read Deprecated: EnumItem,
  read IncorrectIndexType: EnumItem,
  read PluginPermissions: EnumItem,
  read CommandLinePermissions: EnumItem,
  read RobloxPermissions: EnumItem,
  read AddParens: EnumItem,
  read PutCursorInParens: EnumItem,
  read TypeCorrect: EnumItem,
  read ClientServerBoundaryViolation: EnumItem,
  read Invalidated: EnumItem,
  read PutCursorBeforeEnd: EnumItem,
}
type CompletionTriggerKind = {
  read Invoked: EnumItem,
  read TriggerCharacter: EnumItem,
  read TriggerForIncompleteCompletions: EnumItem,
}
type CompositeValueCurveType = {
  read ColorRGB: EnumItem,
  read ColorHSV: EnumItem,
  read NumberRange: EnumItem,
  read Rect: EnumItem,
  read UDim: EnumItem,
  read UDim2: EnumItem,
  read Vector2: EnumItem,
  read Vector3: EnumItem,
}
type CompressionAlgorithm = {
  read Zstd: EnumItem,
}
type ComputerCameraMovementMode = {
  read Default: EnumItem,
  read Classic: EnumItem,
  read Follow: EnumItem,
  read Orbital: EnumItem,
  read CameraToggle: EnumItem,
}
type ComputerMovementMode = {
  read Default: EnumItem,
  read KeyboardMouse: EnumItem,
  read ClickToMove: EnumItem,
}
type ConfigSnapshotErrorState = {
  read None: EnumItem,
  read LoadFailed: EnumItem,
}
type ConnectionError = {
  read OK: EnumItem,
  read Unknown: EnumItem,
  read ConnectErrors: EnumItem,
  read AlreadyConnected: EnumItem,
  read NoFreeIncomingConnections: EnumItem,
  read ConnectionBanned: EnumItem,
  read InvalidPassword: EnumItem,
  read IncompatibleProtocolVersion: EnumItem,
  read IPRecentlyConnected: EnumItem,
  read OurSystemRequiresSecurity: EnumItem,
  read SecurityKeyMismatch: EnumItem,
  read DisconnectErrors: EnumItem,
  read DisconnectBadhash: EnumItem,
  read DisconnectSecurityKeyMismatch: EnumItem,
  read DisconnectProtocolMismatch: EnumItem,
  read DisconnectReceivePacketError: EnumItem,
  read DisconnectReceivePacketStreamError: EnumItem,
  read DisconnectSendPacketError: EnumItem,
  read DisconnectIllegalTeleport: EnumItem,
  read DisconnectDuplicatePlayer: EnumItem,
  read DisconnectDuplicateTicket: EnumItem,
  read DisconnectTimeout: EnumItem,
  read DisconnectLuaKick: EnumItem,
  read DisconnectOnRemoteSysStats: EnumItem,
  read DisconnectHashTimeout: EnumItem,
  read DisconnectCloudEditKick: EnumItem,
  read DisconnectPlayerless: EnumItem,
  read DisconnectNewSecurityKeyMismatch: EnumItem,
  read DisconnectEvicted: EnumItem,
  read DisconnectDevMaintenance: EnumItem,
  read DisconnectRobloxMaintenance: EnumItem,
  read DisconnectRejoin: EnumItem,
  read DisconnectConnectionLost: EnumItem,
  read DisconnectIdle: EnumItem,
  read DisconnectRaknetErrors: EnumItem,
  read DisconnectWrongVersion: EnumItem,
  read DisconnectBySecurityPolicy: EnumItem,
  read DisconnectBlockedIP: EnumItem,
  read DisconnectClientFailure: EnumItem,
  read DisconnectClientRequest: EnumItem,
  read DisconnectPrivateServerKickout: EnumItem,
  read DisconnectModeratedGame: EnumItem,
  read ServerShutdown: EnumItem,
  read ReplicatorTimeout: EnumItem,
  read PlayerRemoved: EnumItem,
  read DisconnectOutOfMemoryKeepPlayingLeave: EnumItem,
  read DisconnectRomarkEndOfTest: EnumItem,
  read DisconnectCollaboratorPermissionRevoked: EnumItem,
  read DisconnectCollaboratorUnderage: EnumItem,
  read NetworkInternal: EnumItem,
  read NetworkSend: EnumItem,
  read NetworkTimeout: EnumItem,
  read NetworkMisbehavior: EnumItem,
  read NetworkSecurity: EnumItem,
  read ReplacementReady: EnumItem,
  read ServerEmpty: EnumItem,
  read PhantomFreeze: EnumItem,
  read AndroidAnticheatKick: EnumItem,
  read AndroidEmulatorKick: EnumItem,
  read AndroidRootedKick: EnumItem,
  read ScreentimeLockoutKick: EnumItem,
  read DisconnectionNotification: EnumItem,
  read DisconnectVerboselyModeratedGame: EnumItem,
  read DisconnectCollaboratorNotAgeVerified: EnumItem,
  read DisconnectCollaboratorTrustedConnectionsRequired: EnumItem,
  read DisconnectCollaboratorOwnerActionRequired: EnumItem,
  read DisconnectCollaboratorTooManyCollaborators: EnumItem,
  read DisconnectCollaboratorUnknownError: EnumItem,
  read DisconnectCollaboratorRequestedEviction: EnumItem,
  read DisconnectCollaboratorTrustedConnectionsRequiredPC: EnumItem,
  read DisconnectRemoteAttestationUnsupported: EnumItem,
  read DisconnectRemoteAttestationGeneralFailure: EnumItem,
  read DisconnectRemoteAttestationTimeout: EnumItem,
  read DisconnectRemoteAttestationOSOutOfDate: EnumItem,
  read DisconnectRemoteAttestationBootValidationFailure: EnumItem,
  read PlacelaunchErrors: EnumItem,
  read PlacelaunchDisabled: EnumItem,
  read PlacelaunchError: EnumItem,
  read PlacelaunchGameEnded: EnumItem,
  read PlacelaunchGameFull: EnumItem,
  read PlacelaunchUserLeft: EnumItem,
  read PlacelaunchRestricted: EnumItem,
  read PlacelaunchUnauthorized: EnumItem,
  read PlacelaunchFlooded: EnumItem,
  read PlacelaunchHashExpired: EnumItem,
  read PlacelaunchHashException: EnumItem,
  read PlacelaunchPartyCannotFit: EnumItem,
  read PlacelaunchHttpError: EnumItem,
  read PlacelaunchUserPrivacyUnauthorized: EnumItem,
  read PlacelaunchVipOwnerNotPresent: EnumItem,
  read PlacelaunchAgeVerificationRequired: EnumItem,
  read PlacelaunchParentalApprovalRequired: EnumItem,
  read PlacelaunchCoreGated: EnumItem,
  read PlacelaunchCollaborationCoreGated: EnumItem,
  read PlacelaunchCreatorBan: EnumItem,
  read PlacelaunchDeviceBlock: EnumItem,
  read PlacelaunchCustomMessage: EnumItem,
  read PlacelaunchOtherError: EnumItem,
  read TeleportErrors: EnumItem,
  read TeleportFailure: EnumItem,
  read TeleportGameNotFound: EnumItem,
  read TeleportGameEnded: EnumItem,
  read TeleportGameFull: EnumItem,
  read TeleportUnauthorized: EnumItem,
  read TeleportFlooded: EnumItem,
  read TeleportIsTeleporting: EnumItem,
}
type ConnectionState = {
  read Connected: EnumItem,
  read Disconnected: EnumItem,
}
type ContentSourceType = {
  read None: EnumItem,
  read Uri: EnumItem,
  read Object: EnumItem,
  read Opaque: EnumItem,
}
type ContextActionPriority = {
  read Low: EnumItem,
  read Medium: EnumItem,
  read High: EnumItem,
}
type ContextActionResult = {
  read Sink: EnumItem,
  read Pass: EnumItem,
}
type ControlMode = {
  read Classic: EnumItem,
  read MouseLockSwitch: EnumItem,
}
type CoreGuiType = {
  read PlayerList: EnumItem,
  read Health: EnumItem,
  read Backpack: EnumItem,
  read Chat: EnumItem,
  read All: EnumItem,
  read EmotesMenu: EnumItem,
  read SelfView: EnumItem,
  read Captures: EnumItem,
  read AvatarSwitcher: EnumItem,
  read ExperienceShop: EnumItem,
}
type CreateAssetResult = {
  read Success: EnumItem,
  read PermissionDenied: EnumItem,
  read UploadFailed: EnumItem,
  read Unknown: EnumItem,
}
type CreateContentResult = {
  read Success: EnumItem,
  read PermissionDenied: EnumItem,
  read UploadFailed: EnumItem,
  read StorageLimitExceeded: EnumItem,
  read Unknown: EnumItem,
}
type CreateOutfitFailure = {
  read InvalidName: EnumItem,
  read OutfitLimitReached: EnumItem,
  read Other: EnumItem,
}
type CreatorType = {
  read User: EnumItem,
  read Group: EnumItem,
}
type CreatorTypeFilter = {
  read User: EnumItem,
  read Group: EnumItem,
  read All: EnumItem,
}
type CurrencyType = {
  read Default: EnumItem,
  read Robux: EnumItem,
  read Tix: EnumItem,
}
type CustomCameraMode = {
  read Default: EnumItem,
  read Classic: EnumItem,
  read Follow: EnumItem,
}
type DataModelExtractorFileType = {
  read PlaceFile: EnumItem,
  read FirstSlice: EnumItem,
  read NonFirstSlice: EnumItem,
}
type DataStoreRequestType = {
  read GetAsync: EnumItem,
  read SetIncrementAsync: EnumItem,
  read UpdateAsync: EnumItem,
  read GetSortedAsync: EnumItem,
  read SetIncrementSortedAsync: EnumItem,
  read OnUpdate: EnumItem,
  read ListAsync: EnumItem,
  read GetVersionAsync: EnumItem,
  read RemoveVersionAsync: EnumItem,
  read StandardRead: EnumItem,
  read StandardWrite: EnumItem,
  read StandardList: EnumItem,
  read StandardRemove: EnumItem,
  read OrderedRead: EnumItem,
  read OrderedWrite: EnumItem,
  read OrderedList: EnumItem,
  read OrderedRemove: EnumItem,
}
type DebugBreakModeType = {
  read Never: EnumItem,
  read Always: EnumItem,
  read Unhandled: EnumItem,
}
type DebuggerEndReason = {
  read ClientRequest: EnumItem,
  read Timeout: EnumItem,
  read InvalidHost: EnumItem,
  read Disconnected: EnumItem,
  read ServerShutdown: EnumItem,
  read ServerProtocolMismatch: EnumItem,
  read ConfigurationFailed: EnumItem,
  read RpcError: EnumItem,
}
type DebuggerExceptionBreakMode = {
  read Never: EnumItem,
  read Always: EnumItem,
  read Unhandled: EnumItem,
}
type DebuggerFrameType = {
  read C: EnumItem,
  read Lua: EnumItem,
}
type DebuggerPauseReason = {
  read Unknown: EnumItem,
  read Requested: EnumItem,
  read Breakpoint: EnumItem,
  read Exception: EnumItem,
  read SingleStep: EnumItem,
  read Entrypoint: EnumItem,
}
type DebuggerResumeType = {
  read StepInto: EnumItem,
  read StepOut: EnumItem,
  read StepOver: EnumItem,
  read Resume: EnumItem,
}
type DebuggerStatus = {
  read Success: EnumItem,
  read Timeout: EnumItem,
  read ConnectionLost: EnumItem,
  read InvalidResponse: EnumItem,
  read InternalError: EnumItem,
  read InvalidState: EnumItem,
  read RpcError: EnumItem,
  read InvalidArgument: EnumItem,
  read ConnectionClosed: EnumItem,
}
type DefaultScriptSyncFileType = {
  read Lua: EnumItem,
  read Luau: EnumItem,
}
type DevCameraOcclusionMode = {
  read Zoom: EnumItem,
  read Invisicam: EnumItem,
}
type DevComputerCameraMovementMode = {
  read UserChoice: EnumItem,
  read Classic: EnumItem,
  read Follow: EnumItem,
  read Orbital: EnumItem,
  read CameraToggle: EnumItem,
}
type DevComputerMovementMode = {
  read UserChoice: EnumItem,
  read KeyboardMouse: EnumItem,
  read ClickToMove: EnumItem,
  read Scriptable: EnumItem,
}
type DevTouchCameraMovementMode = {
  read UserChoice: EnumItem,
  read Classic: EnumItem,
  read Follow: EnumItem,
  read Orbital: EnumItem,
}
type DevTouchMovementMode = {
  read UserChoice: EnumItem,
  read Thumbstick: EnumItem,
  read DPad: EnumItem,
  read Thumbpad: EnumItem,
  read ClickToMove: EnumItem,
  read Scriptable: EnumItem,
  read DynamicThumbstick: EnumItem,
}
type DeveloperMemoryTag = {
  read Internal: EnumItem,
  read HttpCache: EnumItem,
  read Instances: EnumItem,
  read Signals: EnumItem,
  read LuaHeap: EnumItem,
  read Script: EnumItem,
  read PhysicsCollision: EnumItem,
  read BaseParts: EnumItem,
  read GraphicsSolidModels: EnumItem,
  read GraphicsMeshParts: EnumItem,
  read GraphicsParticles: EnumItem,
  read GraphicsParts: EnumItem,
  read GraphicsSpatialHash: EnumItem,
  read GraphicsTerrain: EnumItem,
  read GraphicsTexture: EnumItem,
  read GraphicsTextureCharacter: EnumItem,
  read Sounds: EnumItem,
  read StreamingSounds: EnumItem,
  read TerrainVoxels: EnumItem,
  read Gui: EnumItem,
  read Animation: EnumItem,
  read Navigation: EnumItem,
  read GeometryCSG: EnumItem,
  read GraphicsSlimModels: EnumItem,
}
type DeviceFeatureType = {
  read DeviceCapture: EnumItem,
  read InExperienceFAE: EnumItem,
}
type DeviceForm = {
  read Console: EnumItem,
  read Phone: EnumItem,
  read Tablet: EnumItem,
  read Desktop: EnumItem,
  read VR: EnumItem,
}
type DeviceLevel = {
  read Low: EnumItem,
  read Medium: EnumItem,
  read High: EnumItem,
}
type DeviceSimulatorScalingMode = {
  read ScaleToPhysicalSize: EnumItem,
  read ActualResolution: EnumItem,
  read FitToWindow: EnumItem,
}
type DeviceType = {
  read Unknown: EnumItem,
  read Desktop: EnumItem,
  read Tablet: EnumItem,
  read Phone: EnumItem,
  read TV: EnumItem,
}
type DialogBehaviorType = {
  read SinglePlayer: EnumItem,
  read MultiplePlayers: EnumItem,
}
type DialogPurpose = {
  read Quest: EnumItem,
  read Help: EnumItem,
  read Shop: EnumItem,
}
type DialogTone = {
  read Neutral: EnumItem,
  read Friendly: EnumItem,
  read Enemy: EnumItem,
}
type DigitsRigDescriptionSide = {
  read None: EnumItem,
  read Left: EnumItem,
  read Right: EnumItem,
}
type DiscountType = {
  read Uncategorized: EnumItem,
}
type DisplayScalingMode = {
  read Default: EnumItem,
  read Legacy: EnumItem,
  read Responsive: EnumItem,
}
type DisplaySize = {
  read Small: EnumItem,
  read Medium: EnumItem,
  read Large: EnumItem,
}
type DomainType = {
  read EXPERIENCE: EnumItem,
  read OAUTH: EnumItem,
}
type DominantAxis = {
  read Width: EnumItem,
  read Height: EnumItem,
}
type DraftStatusCode = {
  read OK: EnumItem,
  read DraftOutdated: EnumItem,
  read ScriptRemoved: EnumItem,
  read DraftCommitted: EnumItem,
}
type DragDetectorDragStyle = {
  read TranslateLine: EnumItem,
  read TranslatePlane: EnumItem,
  read TranslatePlaneOrLine: EnumItem,
  read TranslateLineOrPlane: EnumItem,
  read TranslateViewPlane: EnumItem,
  read RotateAxis: EnumItem,
  read RotateTrackball: EnumItem,
  read Scriptable: EnumItem,
  read BestForDevice: EnumItem,
}
type DragDetectorPermissionPolicy = {
  read Nobody: EnumItem,
  read Everybody: EnumItem,
  read Scriptable: EnumItem,
}
type DragDetectorResponseStyle = {
  read Geometric: EnumItem,
  read Physical: EnumItem,
  read Custom: EnumItem,
}
type DraggerCoordinateSpace = {
  read Object: EnumItem,
  read World: EnumItem,
}
type DraggerMovementMode = {
  read Geometric: EnumItem,
  read Physical: EnumItem,
}
type DraggingScrollBar = {
  read None: EnumItem,
  read Horizontal: EnumItem,
  read Vertical: EnumItem,
}
type EasingDirection = {
  read In: EnumItem,
  read Out: EnumItem,
  read InOut: EnumItem,
}
type EasingStyle = {
  read Linear: EnumItem,
  read Sine: EnumItem,
  read Back: EnumItem,
  read Quad: EnumItem,
  read Quart: EnumItem,
  read Quint: EnumItem,
  read Bounce: EnumItem,
  read Elastic: EnumItem,
  read Exponential: EnumItem,
  read Circular: EnumItem,
  read Cubic: EnumItem,
}
type EditableStatus = {
  read Unknown: EnumItem,
  read Allowed: EnumItem,
  read Disallowed: EnumItem,
}
type ElasticBehavior = {
  read WhenScrollable: EnumItem,
  read Always: EnumItem,
  read Never: EnumItem,
}
type EmitterPositionType = {
  read Parent: EnumItem,
  read Instance: EnumItem,
}
type EngagementLevel = {
  read Unknown: EnumItem,
  read Inactive: EnumItem,
  read Low: EnumItem,
  read Medium: EnumItem,
  read High: EnumItem,
}
type EngineFolder = {
  read Screenshots: EnumItem,
  read Videos: EnumItem,
  read Logs: EnumItem,
}
type EnviromentalPhysicsThrottle = {
  read DefaultAuto: EnumItem,
  read Disabled: EnumItem,
  read Always: EnumItem,
  read Skip2: EnumItem,
  read Skip4: EnumItem,
  read Skip8: EnumItem,
  read Skip16: EnumItem,
}
type ExperienceActivationStatus = {
  read Unknown: EnumItem,
  read New: EnumItem,
  read Active: EnumItem,
  read Lapsed: EnumItem,
  read Reactivated: EnumItem,
}
type ExperienceAuthScope = {
  read DefaultScope: EnumItem,
  read CreatorAssetsCreate: EnumItem,
}
type ExperienceEventStatus = {
  read Active: EnumItem,
  read Cancelled: EnumItem,
  read Moderated: EnumItem,
  read Unpublished: EnumItem,
  read Unknown: EnumItem,
}
type ExperienceStateCaptureSelectionMode = {
  read Default: EnumItem,
  read SafetyHighlightMode: EnumItem,
}
type ExperienceStateRecordingLoadMode = {
  read NewReplay: EnumItem,
  read ContiguousSlice: EnumItem,
  read NoncontiguousSlice: EnumItem,
}
type ExperienceStateRecordingLoadSourceType = {
  read S3Url: EnumItem,
  read File: EnumItem,
}
type ExperienceStateRecordingPlaybackMode = {
  read Undefined: EnumItem,
  read Stopped: EnumItem,
  read Playing: EnumItem,
  read Rewinding: EnumItem,
}
type ExplosionType = {
  read NoCraters: EnumItem,
  read Craters: EnumItem,
}
type ExternalEditorMode = {
  read SystemDefault: EnumItem,
  read UserSelectedEditor: EnumItem,
}
type FACSDataLod = {
  read LOD0: EnumItem,
  read LOD1: EnumItem,
  read LODCount: EnumItem,
}
type FacialAgeEstimationResultType = {
  read Complete: EnumItem,
  read Cancel: EnumItem,
  read Error: EnumItem,
}
type FacialAnimationStreamingState = {
  read None: EnumItem,
  read Audio: EnumItem,
  read Video: EnumItem,
  read Place: EnumItem,
  read Server: EnumItem,
}
type FacsActionUnit = {
  read ChinRaiserUpperLip: EnumItem,
  read ChinRaiser: EnumItem,
  read FlatPucker: EnumItem,
  read Funneler: EnumItem,
  read LowerLipSuck: EnumItem,
  read LipPresser: EnumItem,
  read LipsTogether: EnumItem,
  read MouthLeft: EnumItem,
  read MouthRight: EnumItem,
  read Pucker: EnumItem,
  read UpperLipSuck: EnumItem,
  read LeftCheekPuff: EnumItem,
  read LeftDimpler: EnumItem,
  read LeftLipCornerDown: EnumItem,
  read LeftLowerLipDepressor: EnumItem,
  read LeftLipCornerPuller: EnumItem,
  read LeftLipStretcher: EnumItem,
  read LeftUpperLipRaiser: EnumItem,
  read RightCheekPuff: EnumItem,
  read RightDimpler: EnumItem,
  read RightLipCornerDown: EnumItem,
  read RightLowerLipDepressor: EnumItem,
  read RightLipCornerPuller: EnumItem,
  read RightLipStretcher: EnumItem,
  read RightUpperLipRaiser: EnumItem,
  read JawDrop: EnumItem,
  read JawLeft: EnumItem,
  read JawRight: EnumItem,
  read Corrugator: EnumItem,
  read LeftBrowLowerer: EnumItem,
  read LeftOuterBrowRaiser: EnumItem,
  read LeftNoseWrinkler: EnumItem,
  read LeftInnerBrowRaiser: EnumItem,
  read RightBrowLowerer: EnumItem,
  read RightOuterBrowRaiser: EnumItem,
  read RightInnerBrowRaiser: EnumItem,
  read RightNoseWrinkler: EnumItem,
  read EyesLookDown: EnumItem,
  read EyesLookLeft: EnumItem,
  read EyesLookUp: EnumItem,
  read EyesLookRight: EnumItem,
  read LeftCheekRaiser: EnumItem,
  read LeftEyeUpperLidRaiser: EnumItem,
  read LeftEyeClosed: EnumItem,
  read RightCheekRaiser: EnumItem,
  read RightEyeUpperLidRaiser: EnumItem,
  read RightEyeClosed: EnumItem,
  read TongueDown: EnumItem,
  read TongueOut: EnumItem,
  read TongueUp: EnumItem,
}
type FeatureRestrictionAbuseVector = {
  read ExperienceChat: EnumItem,
  read Communication: EnumItem,
}
type FeedbackType = {
  read Feedback: EnumItem,
  read PlayerSupport: EnumItem,
}
type FieldOfViewMode = {
  read Vertical: EnumItem,
  read Diagonal: EnumItem,
  read MaxAxis: EnumItem,
}
type FillDirection = {
  read Horizontal: EnumItem,
  read Vertical: EnumItem,
}
type FilterErrorType = {
  read BackslashNotEscapingAnything: EnumItem,
  read BadBespokeFilter: EnumItem,
  read BadName: EnumItem,
  read IncompleteOr: EnumItem,
  read IncompleteParenthesis: EnumItem,
  read InvalidDoubleStar: EnumItem,
  read InvalidTilde: EnumItem,
  read PropertyBadOperator: EnumItem,
  read PropertyDoesNotExist: EnumItem,
  read PropertyInvalidField: EnumItem,
  read PropertyInvalidValue: EnumItem,
  read PropertyUnsupportedFields: EnumItem,
  read PropertyUnsupportedProperty: EnumItem,
  read UnexpectedNameIndex: EnumItem,
  read UnexpectedToken: EnumItem,
  read UnfinishedBinaryOperator: EnumItem,
  read UnfinishedQuote: EnumItem,
  read UnknownBespokeFilter: EnumItem,
  read WildcardInProperty: EnumItem,
}
type FilterResult = {
  read Accepted: EnumItem,
  read Rejected: EnumItem,
}
type FilterType = {
  read Exclude: EnumItem,
  read Include: EnumItem,
}
type FinishRecordingOperation = {
  read Cancel: EnumItem,
  read Commit: EnumItem,
  read Append: EnumItem,
}
type FluidFidelity = {
  read Automatic: EnumItem,
  read UseCollisionGeometry: EnumItem,
  read UsePreciseGeometry: EnumItem,
}
type FluidForces = {
  read Default: EnumItem,
  read Experimental: EnumItem,
}
type Font = {
  read Legacy: EnumItem,
  read Arial: EnumItem,
  read ArialBold: EnumItem,
  read SourceSans: EnumItem,
  read SourceSansBold: EnumItem,
  read SourceSansLight: EnumItem,
  read SourceSansItalic: EnumItem,
  read Bodoni: EnumItem,
  read Garamond: EnumItem,
  read Cartoon: EnumItem,
  read Code: EnumItem,
  read Highway: EnumItem,
  read SciFi: EnumItem,
  read Arcade: EnumItem,
  read Fantasy: EnumItem,
  read Antique: EnumItem,
  read SourceSansSemibold: EnumItem,
  read Gotham: EnumItem,
  read GothamMedium: EnumItem,
  read GothamBold: EnumItem,
  read GothamBlack: EnumItem,
  read AmaticSC: EnumItem,
  read Bangers: EnumItem,
  read Creepster: EnumItem,
  read DenkOne: EnumItem,
  read Fondamento: EnumItem,
  read FredokaOne: EnumItem,
  read GrenzeGotisch: EnumItem,
  read IndieFlower: EnumItem,
  read JosefinSans: EnumItem,
  read Jura: EnumItem,
  read Kalam: EnumItem,
  read LuckiestGuy: EnumItem,
  read Merriweather: EnumItem,
  read Michroma: EnumItem,
  read Nunito: EnumItem,
  read Oswald: EnumItem,
  read PatrickHand: EnumItem,
  read PermanentMarker: EnumItem,
  read Roboto: EnumItem,
  read RobotoCondensed: EnumItem,
  read RobotoMono: EnumItem,
  read Sarpanch: EnumItem,
  read SpecialElite: EnumItem,
  read TitilliumWeb: EnumItem,
  read Ubuntu: EnumItem,
  read BuilderSans: EnumItem,
  read BuilderSansMedium: EnumItem,
  read BuilderSansBold: EnumItem,
  read BuilderSansExtraBold: EnumItem,
  read Arimo: EnumItem,
  read ArimoBold: EnumItem,
  read Unknown: EnumItem,
}
type FontSize = {
  read Size8: EnumItem,
  read Size9: EnumItem,
  read Size10: EnumItem,
  read Size11: EnumItem,
  read Size12: EnumItem,
  read Size14: EnumItem,
  read Size18: EnumItem,
  read Size24: EnumItem,
  read Size36: EnumItem,
  read Size48: EnumItem,
  read Size28: EnumItem,
  read Size32: EnumItem,
  read Size42: EnumItem,
  read Size60: EnumItem,
  read Size96: EnumItem,
}
type FontStyle = {
  read Normal: EnumItem,
  read Italic: EnumItem,
}
type FontWeight = {
  read Thin: EnumItem,
  read ExtraLight: EnumItem,
  read Light: EnumItem,
  read Regular: EnumItem,
  read Medium: EnumItem,
  read SemiBold: EnumItem,
  read Bold: EnumItem,
  read ExtraBold: EnumItem,
  read Heavy: EnumItem,
}
type ForceLimitMode = {
  read Magnitude: EnumItem,
  read PerAxis: EnumItem,
}
type FormFactor = {
  read Symmetric: EnumItem,
  read Brick: EnumItem,
  read Plate: EnumItem,
  read Custom: EnumItem,
}
type FrameStyle = {
  read Custom: EnumItem,
  read ChatBlue: EnumItem,
  read RobloxSquare: EnumItem,
  read RobloxRound: EnumItem,
  read ChatGreen: EnumItem,
  read ChatRed: EnumItem,
  read DropShadow: EnumItem,
}
type FramerateManagerMode = {
  read Automatic: EnumItem,
  read On: EnumItem,
  read Off: EnumItem,
}
type FriendRequestEvent = {
  read Issue: EnumItem,
  read Revoke: EnumItem,
  read Accept: EnumItem,
  read Deny: EnumItem,
}
type FriendStatus = {
  read Unknown: EnumItem,
  read NotFriend: EnumItem,
  read Friend: EnumItem,
  read FriendRequestSent: EnumItem,
  read FriendRequestReceived: EnumItem,
}
type FunctionalTestResult = {
  read Passed: EnumItem,
  read Warning: EnumItem,
  read Error: EnumItem,
}
type GameAvatarType = {
  read R6: EnumItem,
  read R15: EnumItem,
  read PlayerChoice: EnumItem,
}
type GamepadType = {
  read Unknown: EnumItem,
  read PS4: EnumItem,
  read PS5: EnumItem,
  read XboxOne: EnumItem,
}
type GearGenreSetting = {
  read AllGenres: EnumItem,
  read MatchingGenreOnly: EnumItem,
}
type GearType = {
  read MeleeWeapons: EnumItem,
  read RangedWeapons: EnumItem,
  read Explosives: EnumItem,
  read PowerUps: EnumItem,
  read NavigationEnhancers: EnumItem,
  read MusicalInstruments: EnumItem,
  read SocialItems: EnumItem,
  read BuildingTools: EnumItem,
  read Transport: EnumItem,
}
type Genre = {
  read All: EnumItem,
  read TownAndCity: EnumItem,
  read Fantasy: EnumItem,
  read SciFi: EnumItem,
  read Ninja: EnumItem,
  read Scary: EnumItem,
  read Pirate: EnumItem,
  read Adventure: EnumItem,
  read Sports: EnumItem,
  read Funny: EnumItem,
  read WildWest: EnumItem,
  read War: EnumItem,
  read SkatePark: EnumItem,
  read Tutorial: EnumItem,
}
type GradientTileMode = {
  read Clamp: EnumItem,
  read Repeat: EnumItem,
  read Mirror: EnumItem,
}
type GradientType = {
  read Linear: EnumItem,
  read Radial: EnumItem,
  read Conical: EnumItem,
}
type GraphicsMode = {
  read Automatic: EnumItem,
  read Direct3D11: EnumItem,
  read OpenGL: EnumItem,
  read Metal: EnumItem,
  read Vulkan: EnumItem,
  read NoGraphics: EnumItem,
}
type GraphicsOptimizationMode = {
  read Performance: EnumItem,
  read Balanced: EnumItem,
  read Quality: EnumItem,
}
type GroupMembershipStatus = {
  read None: EnumItem,
  read Joined: EnumItem,
  read JoinRequestPending: EnumItem,
  read AlreadyMember: EnumItem,
}
type GuiState = {
  read Idle: EnumItem,
  read Hover: EnumItem,
  read Press: EnumItem,
  read NonInteractable: EnumItem,
}
type GuiType = {
  read Core: EnumItem,
  read Custom: EnumItem,
  read PlayerNameplates: EnumItem,
  read CustomBillboards: EnumItem,
  read CoreBillboards: EnumItem,
}
type HandlesStyle = {
  read Resize: EnumItem,
  read Movement: EnumItem,
}
type HapticEffectType = {
  read Custom: EnumItem,
  read UIHover: EnumItem,
  read UIClick: EnumItem,
  read UINotification: EnumItem,
  read GameplayExplosion: EnumItem,
  read GameplayCollision: EnumItem,
}
type HashAlgorithm = {
  read Blake2b: EnumItem,
  read Blake3: EnumItem,
  read Md5: EnumItem,
  read Sha1: EnumItem,
  read Sha256: EnumItem,
}
type HighlightDepthMode = {
  read AlwaysOnTop: EnumItem,
  read Occluded: EnumItem,
}
type HorizontalAlignment = {
  read Center: EnumItem,
  read Left: EnumItem,
  read Right: EnumItem,
}
type HoverAnimateSpeed = {
  read VerySlow: EnumItem,
  read Slow: EnumItem,
  read Medium: EnumItem,
  read Fast: EnumItem,
  read VeryFast: EnumItem,
}
type HttpCachePolicy = {
  read None: EnumItem,
  read Full: EnumItem,
  read DataOnly: EnumItem,
  read Default: EnumItem,
  read InternalRedirectRefresh: EnumItem,
}
type HttpCompression = {
  read None: EnumItem,
  read Gzip: EnumItem,
}
type HttpContentType = {
  read ApplicationJson: EnumItem,
  read ApplicationXml: EnumItem,
  read ApplicationUrlEncoded: EnumItem,
  read TextPlain: EnumItem,
  read TextXml: EnumItem,
}
type HttpError = {
  read OK: EnumItem,
  read InvalidUrl: EnumItem,
  read DnsResolve: EnumItem,
  read ConnectFail: EnumItem,
  read OutOfMemory: EnumItem,
  read TimedOut: EnumItem,
  read TooManyRedirects: EnumItem,
  read InvalidRedirect: EnumItem,
  read NetFail: EnumItem,
  read Aborted: EnumItem,
  read SslConnectFail: EnumItem,
  read SslVerificationFail: EnumItem,
  read Unknown: EnumItem,
  read ConnectionClosed: EnumItem,
  read ServerProtocolError: EnumItem,
  read CreatorEnvironmentsNotSupportedByService: EnumItem,
  read InactivityTimeout: EnumItem,
  read TooManyOutstandingRequests: EnumItem,
  read InvalidRangeResponse: EnumItem,
}
type HttpRequestType = {
  read Default: EnumItem,
  read MarketplaceService: EnumItem,
  read Players: EnumItem,
  read Chat: EnumItem,
  read Avatar: EnumItem,
  read Analytics: EnumItem,
  read Localization: EnumItem,
}
type HumanoidCollisionType = {
  read OuterBox: EnumItem,
  read InnerBox: EnumItem,
}
type HumanoidDisplayDistanceType = {
  read Viewer: EnumItem,
  read Subject: EnumItem,
  read None: EnumItem,
}
type HumanoidHealthDisplayType = {
  read DisplayWhenDamaged: EnumItem,
  read AlwaysOn: EnumItem,
  read AlwaysOff: EnumItem,
}
type HumanoidRigType = {
  read R6: EnumItem,
  read R15: EnumItem,
}
type HumanoidStateType = {
  read FallingDown: EnumItem,
  read Ragdoll: EnumItem,
  read GettingUp: EnumItem,
  read Jumping: EnumItem,
  read Swimming: EnumItem,
  read Freefall: EnumItem,
  read Flying: EnumItem,
  read Landed: EnumItem,
  read Running: EnumItem,
  read RunningNoPhysics: EnumItem,
  read StrafingNoPhysics: EnumItem,
  read Climbing: EnumItem,
  read Seated: EnumItem,
  read PlatformStanding: EnumItem,
  read Dead: EnumItem,
  read Physics: EnumItem,
  read None: EnumItem,
}
type IKCollisionsMode = {
  read NoCollisions: EnumItem,
  read OtherMechanismsAnchored: EnumItem,
  read IncludeContactedMechanisms: EnumItem,
}
type IKControlConstraintSupport = {
  read Default: EnumItem,
  read Disabled: EnumItem,
  read Enabled: EnumItem,
}
type IKControlType = {
  read Transform: EnumItem,
  read Position: EnumItem,
  read Rotation: EnumItem,
  read LookAt: EnumItem,
}
type IXPLoadingStatus = {
  read None: EnumItem,
  read Pending: EnumItem,
  read Initialized: EnumItem,
  read ErrorInvalidUser: EnumItem,
  read ErrorConnection: EnumItem,
  read ErrorJsonParse: EnumItem,
  read ErrorTimedOut: EnumItem,
}
type ImageAlphaType = {
  read Default: EnumItem,
  read LockCanvasAlpha: EnumItem,
  read LockCanvasColor: EnumItem,
}
type ImageCombineType = {
  read BlendSourceOver: EnumItem,
  read Overwrite: EnumItem,
  read Add: EnumItem,
  read Multiply: EnumItem,
  read AlphaBlend: EnumItem,
  read NormalMapBlend: EnumItem,
}
type InOut = {
  read Edge: EnumItem,
  read Inset: EnumItem,
  read Center: EnumItem,
}
type InfoType = {
  read Asset: EnumItem,
  read Product: EnumItem,
  read GamePass: EnumItem,
  read Subscription: EnumItem,
  read Bundle: EnumItem,
}
type InitialDockState = {
  read Top: EnumItem,
  read Bottom: EnumItem,
  read Left: EnumItem,
  read Right: EnumItem,
  read Float: EnumItem,
}
type InputActionType = {
  read Bool: EnumItem,
  read Direction1D: EnumItem,
  read Direction2D: EnumItem,
  read Direction3D: EnumItem,
  read ViewportPosition: EnumItem,
}
type InputBindingType = {
  read Automatic: EnumItem,
  read Scriptable: EnumItem,
}
type InputSink = {
  read None: EnumItem,
  read Activate: EnumItem,
  read All: EnumItem,
}
type InputType = {
  read NoInput: EnumItem,
  read Constant: EnumItem,
  read Sin: EnumItem,
}
type InstanceFileSyncStatus = {
  read NotSynced: EnumItem,
  read Errored: EnumItem,
  read SyncedAsRoot: EnumItem,
  read SyncedAsDescendant: EnumItem,
  read AncestorErrored: EnumItem,
}
type IntermediateMeshGenerationResult = {
  read HighQualityMesh: EnumItem,
}
type InternalVideoUsage = {
  read Default: EnumItem,
  read WatchPage: EnumItem,
  read HomeCarousel: EnumItem,
}
type InterpolationThrottlingMode = {
  read Default: EnumItem,
  read Disabled: EnumItem,
  read Enabled: EnumItem,
}
type InviteState = {
  read Placed: EnumItem,
  read Accepted: EnumItem,
  read Declined: EnumItem,
  read Missed: EnumItem,
}
type ItemLineAlignment = {
  read Automatic: EnumItem,
  read Start: EnumItem,
  read Center: EnumItem,
  read End: EnumItem,
  read Stretch: EnumItem,
}
type JoinSource = {
  read CreatedItemAttribution: EnumItem,
}
type JointCreationMode = {
  read All: EnumItem,
  read Surface: EnumItem,
  read None: EnumItem,
}
type KeyCode = {
  read Unknown: EnumItem,
  read Backspace: EnumItem,
  read Tab: EnumItem,
  read Clear: EnumItem,
  read Return: EnumItem,
  read Pause: EnumItem,
  read Escape: EnumItem,
  read Space: EnumItem,
  read QuotedDouble: EnumItem,
  read Hash: EnumItem,
  read Dollar: EnumItem,
  read Percent: EnumItem,
  read Ampersand: EnumItem,
  read Quote: EnumItem,
  read LeftParenthesis: EnumItem,
  read RightParenthesis: EnumItem,
  read Asterisk: EnumItem,
  read Plus: EnumItem,
  read Comma: EnumItem,
  read Minus: EnumItem,
  read Period: EnumItem,
  read Slash: EnumItem,
  read Zero: EnumItem,
  read One: EnumItem,
  read Two: EnumItem,
  read Three: EnumItem,
  read Four: EnumItem,
  read Five: EnumItem,
  read Six: EnumItem,
  read Seven: EnumItem,
  read Eight: EnumItem,
  read Nine: EnumItem,
  read Colon: EnumItem,
  read Semicolon: EnumItem,
  read LessThan: EnumItem,
  read Equals: EnumItem,
  read GreaterThan: EnumItem,
  read Question: EnumItem,
  read At: EnumItem,
  read LeftBracket: EnumItem,
  read BackSlash: EnumItem,
  read RightBracket: EnumItem,
  read Caret: EnumItem,
  read Underscore: EnumItem,
  read Backquote: EnumItem,
  read A: EnumItem,
  read B: EnumItem,
  read C: EnumItem,
  read D: EnumItem,
  read E: EnumItem,
  read F: EnumItem,
  read G: EnumItem,
  read H: EnumItem,
  read I: EnumItem,
  read J: EnumItem,
  read K: EnumItem,
  read L: EnumItem,
  read M: EnumItem,
  read N: EnumItem,
  read O: EnumItem,
  read P: EnumItem,
  read Q: EnumItem,
  read R: EnumItem,
  read S: EnumItem,
  read T: EnumItem,
  read U: EnumItem,
  read V: EnumItem,
  read W: EnumItem,
  read X: EnumItem,
  read Y: EnumItem,
  read Z: EnumItem,
  read LeftCurly: EnumItem,
  read Pipe: EnumItem,
  read RightCurly: EnumItem,
  read Tilde: EnumItem,
  read Delete: EnumItem,
  read World0: EnumItem,
  read World1: EnumItem,
  read World2: EnumItem,
  read World3: EnumItem,
  read World4: EnumItem,
  read World5: EnumItem,
  read World6: EnumItem,
  read World7: EnumItem,
  read World8: EnumItem,
  read World9: EnumItem,
  read World10: EnumItem,
  read World11: EnumItem,
  read World12: EnumItem,
  read World13: EnumItem,
  read World14: EnumItem,
  read World15: EnumItem,
  read World16: EnumItem,
  read World17: EnumItem,
  read World18: EnumItem,
  read World19: EnumItem,
  read World20: EnumItem,
  read World21: EnumItem,
  read World22: EnumItem,
  read World23: EnumItem,
  read World24: EnumItem,
  read World25: EnumItem,
  read World26: EnumItem,
  read World27: EnumItem,
  read World28: EnumItem,
  read World29: EnumItem,
  read World30: EnumItem,
  read World31: EnumItem,
  read World32: EnumItem,
  read World33: EnumItem,
  read World34: EnumItem,
  read World35: EnumItem,
  read World36: EnumItem,
  read World37: EnumItem,
  read World38: EnumItem,
  read World39: EnumItem,
  read World40: EnumItem,
  read World41: EnumItem,
  read World42: EnumItem,
  read World43: EnumItem,
  read World44: EnumItem,
  read World45: EnumItem,
  read World46: EnumItem,
  read World47: EnumItem,
  read World48: EnumItem,
  read World49: EnumItem,
  read World50: EnumItem,
  read World51: EnumItem,
  read World52: EnumItem,
  read World53: EnumItem,
  read World54: EnumItem,
  read World55: EnumItem,
  read World56: EnumItem,
  read World57: EnumItem,
  read World58: EnumItem,
  read World59: EnumItem,
  read World60: EnumItem,
  read World61: EnumItem,
  read World62: EnumItem,
  read World63: EnumItem,
  read World64: EnumItem,
  read World65: EnumItem,
  read World66: EnumItem,
  read World67: EnumItem,
  read World68: EnumItem,
  read World69: EnumItem,
  read World70: EnumItem,
  read World71: EnumItem,
  read World72: EnumItem,
  read World73: EnumItem,
  read World74: EnumItem,
  read World75: EnumItem,
  read World76: EnumItem,
  read World77: EnumItem,
  read World78: EnumItem,
  read World79: EnumItem,
  read World80: EnumItem,
  read World81: EnumItem,
  read World82: EnumItem,
  read World83: EnumItem,
  read World84: EnumItem,
  read World85: EnumItem,
  read World86: EnumItem,
  read World87: EnumItem,
  read World88: EnumItem,
  read World89: EnumItem,
  read World90: EnumItem,
  read World91: EnumItem,
  read World92: EnumItem,
  read World93: EnumItem,
  read World94: EnumItem,
  read World95: EnumItem,
  read KeypadZero: EnumItem,
  read KeypadOne: EnumItem,
  read KeypadTwo: EnumItem,
  read KeypadThree: EnumItem,
  read KeypadFour: EnumItem,
  read KeypadFive: EnumItem,
  read KeypadSix: EnumItem,
  read KeypadSeven: EnumItem,
  read KeypadEight: EnumItem,
  read KeypadNine: EnumItem,
  read KeypadPeriod: EnumItem,
  read KeypadDivide: EnumItem,
  read KeypadMultiply: EnumItem,
  read KeypadMinus: EnumItem,
  read KeypadPlus: EnumItem,
  read KeypadEnter: EnumItem,
  read KeypadEquals: EnumItem,
  read Up: EnumItem,
  read Down: EnumItem,
  read Right: EnumItem,
  read Left: EnumItem,
  read Insert: EnumItem,
  read Home: EnumItem,
  read End: EnumItem,
  read PageUp: EnumItem,
  read PageDown: EnumItem,
  read F1: EnumItem,
  read F2: EnumItem,
  read F3: EnumItem,
  read F4: EnumItem,
  read F5: EnumItem,
  read F6: EnumItem,
  read F7: EnumItem,
  read F8: EnumItem,
  read F9: EnumItem,
  read F10: EnumItem,
  read F11: EnumItem,
  read F12: EnumItem,
  read F13: EnumItem,
  read F14: EnumItem,
  read F15: EnumItem,
  read NumLock: EnumItem,
  read CapsLock: EnumItem,
  read ScrollLock: EnumItem,
  read RightShift: EnumItem,
  read LeftShift: EnumItem,
  read RightControl: EnumItem,
  read LeftControl: EnumItem,
  read RightAlt: EnumItem,
  read LeftAlt: EnumItem,
  read RightMeta: EnumItem,
  read LeftMeta: EnumItem,
  read LeftSuper: EnumItem,
  read RightSuper: EnumItem,
  read Mode: EnumItem,
  read Compose: EnumItem,
  read Help: EnumItem,
  read Print: EnumItem,
  read SysReq: EnumItem,
  read Break: EnumItem,
  read Menu: EnumItem,
  read Power: EnumItem,
  read Euro: EnumItem,
  read Undo: EnumItem,
  read ButtonX: EnumItem,
  read ButtonY: EnumItem,
  read ButtonA: EnumItem,
  read ButtonB: EnumItem,
  read ButtonR1: EnumItem,
  read ButtonL1: EnumItem,
  read ButtonR2: EnumItem,
  read ButtonL2: EnumItem,
  read ButtonR3: EnumItem,
  read ButtonL3: EnumItem,
  read ButtonStart: EnumItem,
  read ButtonSelect: EnumItem,
  read DPadLeft: EnumItem,
  read DPadRight: EnumItem,
  read DPadUp: EnumItem,
  read DPadDown: EnumItem,
  read Thumbstick1: EnumItem,
  read Thumbstick2: EnumItem,
  read Thumbstick1Up: EnumItem,
  read Thumbstick1Down: EnumItem,
  read Thumbstick1Left: EnumItem,
  read Thumbstick1Right: EnumItem,
  read Thumbstick2Up: EnumItem,
  read Thumbstick2Down: EnumItem,
  read Thumbstick2Left: EnumItem,
  read Thumbstick2Right: EnumItem,
  read MouseLeftButton: EnumItem,
  read MouseRightButton: EnumItem,
  read MouseMiddleButton: EnumItem,
  read MouseBackButton: EnumItem,
  read MouseNoButton: EnumItem,
  read MouseX: EnumItem,
  read MouseY: EnumItem,
  read MousePosition: EnumItem,
  read TouchPosition: EnumItem,
  read MouseWheel: EnumItem,
  read TrackpadPan: EnumItem,
  read TrackpadPinch: EnumItem,
  read MouseDelta: EnumItem,
  read TouchDelta: EnumItem,
  read TouchPinch: EnumItem,
}
type KeyCodeStringFormat = {
  read Default: EnumItem,
  read Abbreviated: EnumItem,
}
type KeyInterpolationMode = {
  read Constant: EnumItem,
  read Linear: EnumItem,
  read Cubic: EnumItem,
}
type KeywordFilterType = {
  read Include: EnumItem,
  read Exclude: EnumItem,
}
type Language = {
  read Default: EnumItem,
}
type LeftRight = {
  read Left: EnumItem,
  read Center: EnumItem,
  read Right: EnumItem,
}
type LexemeType = {
  read Eof: EnumItem,
  read Name: EnumItem,
  read QuotedString: EnumItem,
  read Number: EnumItem,
  read And: EnumItem,
  read Or: EnumItem,
  read Equal: EnumItem,
  read TildeEqual: EnumItem,
  read GreaterThan: EnumItem,
  read GreaterThanEqual: EnumItem,
  read LessThan: EnumItem,
  read LessThanEqual: EnumItem,
  read Colon: EnumItem,
  read Dot: EnumItem,
  read LeftParenthesis: EnumItem,
  read RightParenthesis: EnumItem,
  read Star: EnumItem,
  read DoubleStar: EnumItem,
  read ReservedSpecial: EnumItem,
}
type LightingStyle = {
  read Realistic: EnumItem,
  read Soft: EnumItem,
}
type Limb = {
  read Head: EnumItem,
  read Torso: EnumItem,
  read LeftArm: EnumItem,
  read RightArm: EnumItem,
  read LeftLeg: EnumItem,
  read RightLeg: EnumItem,
  read Unknown: EnumItem,
}
type LineJoinMode = {
  read Round: EnumItem,
  read Bevel: EnumItem,
  read Miter: EnumItem,
}
type ListDisplayMode = {
  read Horizontal: EnumItem,
  read Vertical: EnumItem,
}
type ListenerLocation = {
  read Default: EnumItem,
  read None: EnumItem,
  read Character: EnumItem,
  read Camera: EnumItem,
}
type ListenerPositionType = {
  read Parent: EnumItem,
  read Instance: EnumItem,
}
type ListenerType = {
  read Camera: EnumItem,
  read CFrame: EnumItem,
  read ObjectPosition: EnumItem,
  read ObjectCFrame: EnumItem,
}
type LiveEditingAtomicUpdateResponse = {
  read Success: EnumItem,
  read FailureGuidNotFound: EnumItem,
  read FailureHashMismatch: EnumItem,
  read FailureOperationIllegal: EnumItem,
}
type LiveEditingBroadcastMessageType = {
  read Normal: EnumItem,
  read Warning: EnumItem,
  read Error: EnumItem,
}
type LoadCharacterLayeredClothing = {
  read Default: EnumItem,
  read Disabled: EnumItem,
  read Enabled: EnumItem,
}
type LoadDynamicHeads = {
  read Default: EnumItem,
  read Disabled: EnumItem,
  read Enabled: EnumItem,
}
type LocationType = {
  read Character: EnumItem,
  read Camera: EnumItem,
  read ObjectPosition: EnumItem,
}
type LuauTypeCheckMode = {
  read Default: EnumItem,
  read NoCheck: EnumItem,
  read Nonstrict: EnumItem,
  read Strict: EnumItem,
}
type MakeupType = {
  read Face: EnumItem,
  read Lip: EnumItem,
  read Eye: EnumItem,
}
type MarketplaceBulkPurchasePromptStatus = {
  read Completed: EnumItem,
  read Aborted: EnumItem,
  read Error: EnumItem,
}
type MarketplaceItemPurchaseStatus = {
  read Success: EnumItem,
  read SystemError: EnumItem,
  read AlreadyOwned: EnumItem,
  read InsufficientRobux: EnumItem,
  read QuantityLimitExceeded: EnumItem,
  read QuotaExceeded: EnumItem,
  read NotForSale: EnumItem,
  read NotAvailableForPurchaser: EnumItem,
  read PriceMismatch: EnumItem,
  read SoldOut: EnumItem,
  read PurchaserIsSeller: EnumItem,
  read InsufficientMembership: EnumItem,
  read PlaceInvalid: EnumItem,
}
type MarketplaceProductType = {
  read AvatarAsset: EnumItem,
  read AvatarBundle: EnumItem,
}
type MarkupKind = {
  read PlainText: EnumItem,
  read Markdown: EnumItem,
}
type MatchmakingType = {
  read Default: EnumItem,
  read XboxOnly: EnumItem,
  read PlayStationOnly: EnumItem,
}
type Material = {
  read Plastic: EnumItem,
  read SmoothPlastic: EnumItem,
  read Neon: EnumItem,
  read Wood: EnumItem,
  read WoodPlanks: EnumItem,
  read Marble: EnumItem,
  read Basalt: EnumItem,
  read Slate: EnumItem,
  read CrackedLava: EnumItem,
  read Concrete: EnumItem,
  read Limestone: EnumItem,
  read Granite: EnumItem,
  read Pavement: EnumItem,
  read Brick: EnumItem,
  read Pebble: EnumItem,
  read Cobblestone: EnumItem,
  read Rock: EnumItem,
  read Sandstone: EnumItem,
  read CorrodedMetal: EnumItem,
  read DiamondPlate: EnumItem,
  read Foil: EnumItem,
  read Metal: EnumItem,
  read Grass: EnumItem,
  read LeafyGrass: EnumItem,
  read Sand: EnumItem,
  read Fabric: EnumItem,
  read Snow: EnumItem,
  read Mud: EnumItem,
  read Ground: EnumItem,
  read Asphalt: EnumItem,
  read Salt: EnumItem,
  read Ice: EnumItem,
  read Glacier: EnumItem,
  read Glass: EnumItem,
  read ForceField: EnumItem,
  read Air: EnumItem,
  read Water: EnumItem,
  read Cardboard: EnumItem,
  read Carpet: EnumItem,
  read CeramicTiles: EnumItem,
  read ClayRoofTiles: EnumItem,
  read RoofShingles: EnumItem,
  read Leather: EnumItem,
  read Plaster: EnumItem,
  read Rubber: EnumItem,
}
type MaterialPattern = {
  read Regular: EnumItem,
  read Organic: EnumItem,
}
type MembershipType = {
  read None: EnumItem,
  read BuildersClub: EnumItem,
  read TurboBuildersClub: EnumItem,
  read OutrageousBuildersClub: EnumItem,
  read Premium: EnumItem,
}
type MeshPartDetailLevel = {
  read DistanceBased: EnumItem,
  read Level00: EnumItem,
  read Level01: EnumItem,
  read Level02: EnumItem,
  read Level03: EnumItem,
  read Level04: EnumItem,
  read Level05: EnumItem,
  read Level06: EnumItem,
  read Level07: EnumItem,
  read Level08: EnumItem,
  read Level09: EnumItem,
}
type MeshPartHeadsAndAccessories = {
  read Default: EnumItem,
  read Disabled: EnumItem,
  read Enabled: EnumItem,
}
type MeshScaleUnit = {
  read Stud: EnumItem,
  read Meter: EnumItem,
  read CM: EnumItem,
  read MM: EnumItem,
  read Foot: EnumItem,
  read Inch: EnumItem,
}
type MeshType = {
  read Head: EnumItem,
  read Torso: EnumItem,
  read Wedge: EnumItem,
  read Sphere: EnumItem,
  read Cylinder: EnumItem,
  read FileMesh: EnumItem,
  read Brick: EnumItem,
  read Prism: EnumItem,
  read Pyramid: EnumItem,
  read ParallelRamp: EnumItem,
  read RightAngleRamp: EnumItem,
  read CornerWedge: EnumItem,
}
type MessageType = {
  read MessageOutput: EnumItem,
  read MessageInfo: EnumItem,
  read MessageWarning: EnumItem,
  read MessageError: EnumItem,
}
type ModelLevelOfDetail = {
  read Automatic: EnumItem,
  read StreamingMesh: EnumItem,
  read Disabled: EnumItem,
  read SLIM: EnumItem,
}
type ModelStreamingBehavior = {
  read Default: EnumItem,
  read Legacy: EnumItem,
  read Improved: EnumItem,
}
type ModelStreamingMode = {
  read Default: EnumItem,
  read Atomic: EnumItem,
  read Persistent: EnumItem,
  read PersistentPerPlayer: EnumItem,
  read Nonatomic: EnumItem,
}
type ModerationResultCategory = {
  read ViolationDetected: EnumItem,
  read Borderline: EnumItem,
  read NoViolationDetected: EnumItem,
}
type ModerationResultLabel = {
  read ChildExploitation: EnumItem,
  read SuicideSelfInjuryAndHarmfulBehavior: EnumItem,
  read ThreatsBullyingAndHarassment: EnumItem,
  read TerrorismAndViolentExtremism: EnumItem,
  read DiscriminationSlursAndHateSpeech: EnumItem,
  read RealWorldSensitiveEvents: EnumItem,
  read ViolentContentAndGore: EnumItem,
  read RomanticAndSexualContent: EnumItem,
  read IllegalAndRegulatedGoodsAndActivities: EnumItem,
  read Profanity: EnumItem,
  read Other: EnumItem,
}
type ModerationStatus = {
  read ReviewedApproved: EnumItem,
  read ReviewedRejected: EnumItem,
  read NotReviewed: EnumItem,
  read NotApplicable: EnumItem,
  read Invalid: EnumItem,
}
type ModifierKey = {
  read Shift: EnumItem,
  read Ctrl: EnumItem,
  read Alt: EnumItem,
  read Meta: EnumItem,
}
type MouseBehavior = {
  read Default: EnumItem,
  read LockCenter: EnumItem,
  read LockCurrentPosition: EnumItem,
}
type MoveState = {
  read Stopped: EnumItem,
  read Coasting: EnumItem,
  read Pushing: EnumItem,
  read Stopping: EnumItem,
  read AirFree: EnumItem,
}
type MuteState = {
  read Unmuted: EnumItem,
  read Muted: EnumItem,
}
type NameOcclusion = {
  read NoOcclusion: EnumItem,
  read EnemyOcclusion: EnumItem,
  read OccludeAll: EnumItem,
}
type NegateOperationHiddenHistory = {
  read None: EnumItem,
  read NegatedUnion: EnumItem,
  read NegatedIntersection: EnumItem,
}
type NetworkOwnership = {
  read Automatic: EnumItem,
  read Manual: EnumItem,
  read OnContact: EnumItem,
}
type NetworkStatus = {
  read Unknown: EnumItem,
  read Connected: EnumItem,
  read Disconnected: EnumItem,
}
type NoiseType = {
  read SimplexGabor: EnumItem,
}
type NormalId = {
  read Right: EnumItem,
  read Top: EnumItem,
  read Back: EnumItem,
  read Left: EnumItem,
  read Bottom: EnumItem,
  read Front: EnumItem,
}
type NotificationButtonType = {
  read Primary: EnumItem,
  read Secondary: EnumItem,
}
type OperationType = {
  read Null: EnumItem,
  read Union: EnumItem,
  read Subtraction: EnumItem,
  read Intersection: EnumItem,
  read Primitive: EnumItem,
}
type OrientationAlignmentMode = {
  read OneAttachment: EnumItem,
  read TwoAttachment: EnumItem,
}
type OutfitSource = {
  read All: EnumItem,
  read Created: EnumItem,
  read Purchased: EnumItem,
}
type OutfitType = {
  read All: EnumItem,
  read Avatar: EnumItem,
  read DynamicHead: EnumItem,
  read Shoes: EnumItem,
  read Makeup: EnumItem,
}
type OutputLayoutMode = {
  read Horizontal: EnumItem,
  read Vertical: EnumItem,
}
type OverrideMouseIconBehavior = {
  read None: EnumItem,
  read ForceShow: EnumItem,
  read ForceHide: EnumItem,
}
type PackagePermission = {
  read None: EnumItem,
  read NoAccess: EnumItem,
  read Revoked: EnumItem,
  read UseView: EnumItem,
  read Edit: EnumItem,
  read Own: EnumItem,
}
type PartType = {
  read Ball: EnumItem,
  read Block: EnumItem,
  read Cylinder: EnumItem,
  read Wedge: EnumItem,
  read CornerWedge: EnumItem,
}
type ParticleEmitterShape = {
  read Box: EnumItem,
  read Sphere: EnumItem,
  read Cylinder: EnumItem,
  read Disc: EnumItem,
}
type ParticleEmitterShapeInOut = {
  read Outward: EnumItem,
  read Inward: EnumItem,
  read InAndOut: EnumItem,
}
type ParticleEmitterShapeStyle = {
  read Volume: EnumItem,
  read Surface: EnumItem,
}
type ParticleFlipbookLayout = {
  read None: EnumItem,
  read Grid2x2: EnumItem,
  read Grid4x4: EnumItem,
  read Grid8x8: EnumItem,
  read Custom: EnumItem,
}
type ParticleFlipbookMode = {
  read Loop: EnumItem,
  read OneShot: EnumItem,
  read PingPong: EnumItem,
  read Random: EnumItem,
}
type ParticleFlipbookTextureCompatible = {
  read NotCompatible: EnumItem,
  read Compatible: EnumItem,
  read Unknown: EnumItem,
}
type ParticleOrientation = {
  read FacingCamera: EnumItem,
  read FacingCameraWorldUp: EnumItem,
  read VelocityParallel: EnumItem,
  read VelocityPerpendicular: EnumItem,
}
type PathStatus = {
  read Success: EnumItem,
  read ClosestNoPath: EnumItem,
  read ClosestOutOfRange: EnumItem,
  read FailStartNotEmpty: EnumItem,
  read FailFinishNotEmpty: EnumItem,
  read NoPath: EnumItem,
}
type PathWaypointAction = {
  read Walk: EnumItem,
  read Jump: EnumItem,
  read Custom: EnumItem,
}
type PathfindingUseImprovedSearch = {
  read Default: EnumItem,
  read Disabled: EnumItem,
  read Enabled: EnumItem,
}
type PeoplePageLayout = {
  read Card: EnumItem,
  read List: EnumItem,
}
type PerformanceOverlayMode = {
  read Overdraw: EnumItem,
  read Transparent: EnumItem,
  read Decals: EnumItem,
  read Lights: EnumItem,
}
type PermissionLevelShown = {
  read Game: EnumItem,
  read RobloxGame: EnumItem,
  read RobloxScript: EnumItem,
  read Studio: EnumItem,
  read Roblox: EnumItem,
}
type PhysicalConstraintType = {
  read AnimationConstraint: EnumItem,
  read Motor6D: EnumItem,
}
type PhysicsSimulationRate = {
  read Fixed240Hz: EnumItem,
  read Fixed120Hz: EnumItem,
  read Fixed60Hz: EnumItem,
}
type PhysicsSteppingMethod = {
  read Default: EnumItem,
  read Fixed: EnumItem,
  read Adaptive: EnumItem,
}
type PlaceContentPreference = {
  read None: EnumItem,
  read All: EnumItem,
  read MentionsAndReplies: EnumItem,
  read Unknown: EnumItem,
}
type PlacePublishType = {
  read None: EnumItem,
  read Publish: EnumItem,
  read Save: EnumItem,
}
type Platform = {
  read Windows: EnumItem,
  read OSX: EnumItem,
  read IOS: EnumItem,
  read Android: EnumItem,
  read XBoxOne: EnumItem,
  read PS4: EnumItem,
  read PS3: EnumItem,
  read XBox360: EnumItem,
  read WiiU: EnumItem,
  read NX: EnumItem,
  read Ouya: EnumItem,
  read AndroidTV: EnumItem,
  read Chromecast: EnumItem,
  read Linux: EnumItem,
  read SteamOS: EnumItem,
  read WebOS: EnumItem,
  read DOS: EnumItem,
  read BeOS: EnumItem,
  read UWP: EnumItem,
  read PS5: EnumItem,
  read MetaOS: EnumItem,
  read Web: EnumItem,
  read None: EnumItem,
}
type PlaybackState = {
  read Begin: EnumItem,
  read Delayed: EnumItem,
  read Playing: EnumItem,
  read Paused: EnumItem,
  read Completed: EnumItem,
  read Cancelled: EnumItem,
}
type PlayerActions = {
  read CharacterForward: EnumItem,
  read CharacterBackward: EnumItem,
  read CharacterLeft: EnumItem,
  read CharacterRight: EnumItem,
  read CharacterJump: EnumItem,
}
type PlayerCharacterDestroyBehavior = {
  read Default: EnumItem,
  read Disabled: EnumItem,
  read Enabled: EnumItem,
}
type PlayerChatType = {
  read All: EnumItem,
  read Team: EnumItem,
  read Whisper: EnumItem,
}
type PlayerDataErrorState = {
  read LoadFailed: EnumItem,
  read FlushFailed: EnumItem,
  read ReleaseFailed: EnumItem,
  read None: EnumItem,
}
type PlayerDataLoadFailureBehavior = {
  read Failure: EnumItem,
  read FallbackToDefault: EnumItem,
  read Kick: EnumItem,
}
type PlayerExitReason = {
  read Unknown: EnumItem,
  read PlatformKick: EnumItem,
  read CreatorKick: EnumItem,
}
type PlayerPlatformActivationStatus = {
  read Unknown: EnumItem,
  read New: EnumItem,
  read Active: EnumItem,
  read Lapsed: EnumItem,
  read Reactivated: EnumItem,
}
type PlayerPlatformSpenderStatus = {
  read Unknown: EnumItem,
  read Active: EnumItem,
  read OtherPayer: EnumItem,
}
type PluginConnectionTargetType = {
  read Edit: EnumItem,
  read Test: EnumItem,
}
type PoseEasingDirection = {
  read In: EnumItem,
  read Out: EnumItem,
  read InOut: EnumItem,
}
type PoseEasingStyle = {
  read Linear: EnumItem,
  read Constant: EnumItem,
  read Elastic: EnumItem,
  read Cubic: EnumItem,
  read Bounce: EnumItem,
  read CubicV2: EnumItem,
}
type PositionAlignmentMode = {
  read OneAttachment: EnumItem,
  read TwoAttachment: EnumItem,
}
type PredictionMode = {
  read Automatic: EnumItem,
  read On: EnumItem,
  read Off: EnumItem,
}
type PredictionStatus = {
  read Authoritative: EnumItem,
  read Predicted: EnumItem,
  read None: EnumItem,
}
type PredictiveStreamingMode = {
  read Default: EnumItem,
  read Enabled: EnumItem,
  read Disabled: EnumItem,
}
type PreferredInput = {
  read KeyboardAndMouse: EnumItem,
  read Gamepad: EnumItem,
  read Touch: EnumItem,
}
type PreferredTextSize = {
  read Medium: EnumItem,
  read Large: EnumItem,
  read Larger: EnumItem,
  read Largest: EnumItem,
}
type PrefetchDownloadStatus = {
  read NotStarted: EnumItem,
  read InProgress: EnumItem,
  read Completed: EnumItem,
  read Failed: EnumItem,
}
type PrimalPhysicsSolver = {
  read Default: EnumItem,
  read Experimental: EnumItem,
  read Disabled: EnumItem,
}
type PrimitiveType = {
  read Null: EnumItem,
  read Ball: EnumItem,
  read Cylinder: EnumItem,
  read Block: EnumItem,
  read Wedge: EnumItem,
  read CornerWedge: EnumItem,
}
type PrivilegeType = {
  read Banned: EnumItem,
  read Visitor: EnumItem,
  read Member: EnumItem,
  read Admin: EnumItem,
  read Owner: EnumItem,
}
type ProductLocationRestriction = {
  read AvatarShop: EnumItem,
  read AllowedGames: EnumItem,
  read AllGames: EnumItem,
}
type ProductPurchaseChannel = {
  read InExperience: EnumItem,
  read ExperienceDetailsPage: EnumItem,
  read AdReward: EnumItem,
  read CommerceProduct: EnumItem,
}
type ProductPurchaseDecision = {
  read NotProcessedYet: EnumItem,
  read PurchaseGranted: EnumItem,
}
type PromptCreateAssetResult = {
  read Success: EnumItem,
  read PermissionDenied: EnumItem,
  read Timeout: EnumItem,
  read UploadFailed: EnumItem,
  read NoUserInput: EnumItem,
  read UnknownFailure: EnumItem,
  read UGCValidationFailed: EnumItem,
  read ModeratedName: EnumItem,
  read PurchaseFailure: EnumItem,
  read TokenInvalid: EnumItem,
}
type PromptCreateAvatarResult = {
  read Success: EnumItem,
  read PermissionDenied: EnumItem,
  read Timeout: EnumItem,
  read UploadFailed: EnumItem,
  read NoUserInput: EnumItem,
  read InvalidHumanoidDescription: EnumItem,
  read UGCValidationFailed: EnumItem,
  read ModeratedName: EnumItem,
  read MaxOutfits: EnumItem,
  read PurchaseFailure: EnumItem,
  read UnknownFailure: EnumItem,
  read TokenInvalid: EnumItem,
}
type PromptCreateOutfitResult = {
  read Success: EnumItem,
  read PartialSuccess: EnumItem,
  read CreationFailure: EnumItem,
  read PermissionDenied: EnumItem,
  read Timeout: EnumItem,
  read NoUserInput: EnumItem,
  read UnknownFailure: EnumItem,
}
type PromptExperienceDetailsResult = {
  read PromptClosed: EnumItem,
  read TeleportAttempted: EnumItem,
}
type PromptLinkSharingResult = {
  read Success: EnumItem,
  read PlayerLeft: EnumItem,
  read InvalidLaunchData: EnumItem,
}
type PromptPublishAssetResult = {
  read Success: EnumItem,
  read PermissionDenied: EnumItem,
  read Timeout: EnumItem,
  read UploadFailed: EnumItem,
  read NoUserInput: EnumItem,
  read UnknownFailure: EnumItem,
}
type PropertyStatus = {
  read Ok: EnumItem,
  read Warning: EnumItem,
  read Error: EnumItem,
}
type ProximityPromptExclusivity = {
  read OnePerButton: EnumItem,
  read OneGlobally: EnumItem,
  read AlwaysShow: EnumItem,
}
type ProximityPromptInputType = {
  read Keyboard: EnumItem,
  read Gamepad: EnumItem,
  read Touch: EnumItem,
}
type ProximityPromptStyle = {
  read Default: EnumItem,
  read Custom: EnumItem,
}
type PurchaseOption = {
  read TimedOption: EnumItem,
  read Permanent: EnumItem,
}
type QualityLevel = {
  read Automatic: EnumItem,
  read Level01: EnumItem,
  read Level02: EnumItem,
  read Level03: EnumItem,
  read Level04: EnumItem,
  read Level05: EnumItem,
  read Level06: EnumItem,
  read Level07: EnumItem,
  read Level08: EnumItem,
  read Level09: EnumItem,
  read Level10: EnumItem,
  read Level11: EnumItem,
  read Level12: EnumItem,
  read Level13: EnumItem,
  read Level14: EnumItem,
  read Level15: EnumItem,
  read Level16: EnumItem,
  read Level17: EnumItem,
  read Level18: EnumItem,
  read Level19: EnumItem,
  read Level20: EnumItem,
  read Level21: EnumItem,
}
type R15CollisionType = {
  read OuterBox: EnumItem,
  read InnerBox: EnumItem,
}
type RaycastFilterType = {
  read Exclude: EnumItem,
  read Include: EnumItem,
}
type ReadCapturesFromGalleryResult = {
  read Success: EnumItem,
  read NeedPermission: EnumItem,
}
type ReceiptDecision = {
  read NotProcessedYet: EnumItem,
  read Processed: EnumItem,
}
type ReceiptType = {
  read DeveloperProduct: EnumItem,
  read RobuxTransferSender: EnumItem,
  read RobuxTransferReceiver: EnumItem,
}
type RecommendationActionType = {
  read AddReaction: EnumItem,
  read RemoveReaction: EnumItem,
  read Share: EnumItem,
  read Report: EnumItem,
  read Comment: EnumItem,
  read Play: EnumItem,
  read Purchase: EnumItem,
}
type RecommendationDepartureIntent = {
  read Neutral: EnumItem,
  read Positive: EnumItem,
  read Negative: EnumItem,
}
type RecommendationImpressionType = {
  read View: EnumItem,
  read NotViewable: EnumItem,
}
type RecommendationItemContentType = {
  read Static: EnumItem,
  read Dynamic: EnumItem,
  read Interactive: EnumItem,
}
type RecommendationItemVisibility = {
  read Private: EnumItem,
  read Public: EnumItem,
}
type RecommendationPreferenceTargetType = {
  read User: EnumItem,
  read Universe: EnumItem,
  read CustomTag: EnumItem,
}
type RecommendationPreferenceType = {
  read AddFollow: EnumItem,
  read RemoveFollow: EnumItem,
  read AddMute: EnumItem,
  read RemoveMute: EnumItem,
}
type RejectCharacterDeletions = {
  read Default: EnumItem,
  read Disabled: EnumItem,
  read Enabled: EnumItem,
}
type RenderFidelity = {
  read Automatic: EnumItem,
  read Precise: EnumItem,
  read Performance: EnumItem,
}
type RenderPriority = {
  read First: EnumItem,
  read Input: EnumItem,
  read Camera: EnumItem,
  read Character: EnumItem,
  read Last: EnumItem,
}
type RenderingCacheOptimizationMode = {
  read Default: EnumItem,
  read Disabled: EnumItem,
  read Enabled: EnumItem,
}
type RenderingTestComparisonMethod = {
  read psnr: EnumItem,
  read diff: EnumItem,
}
type ReplicateInstanceDestroySetting = {
  read Default: EnumItem,
  read Disabled: EnumItem,
  read Enabled: EnumItem,
}
type ResamplerMode = {
  read Default: EnumItem,
  read Pixelated: EnumItem,
}
type ReservedHighlightId = {
  read Standard: EnumItem,
  read Active: EnumItem,
  read Hover: EnumItem,
  read Selection: EnumItem,
  read NegatedPart: EnumItem,
}
type RestPose = {
  read Default: EnumItem,
  read RotationsReset: EnumItem,
  read Custom: EnumItem,
}
type RestPoseModel = {
  read FromRigInACE: EnumItem,
  read FromRigInFile: EnumItem,
  read FromRigInFileZeroedRotations: EnumItem,
  read FromCustomClip: EnumItem,
}
type ReturnKeyType = {
  read Default: EnumItem,
  read Done: EnumItem,
  read Go: EnumItem,
  read Next: EnumItem,
  read Search: EnumItem,
  read Send: EnumItem,
}
type ReverbType = {
  read NoReverb: EnumItem,
  read GenericReverb: EnumItem,
  read PaddedCell: EnumItem,
  read Room: EnumItem,
  read Bathroom: EnumItem,
  read LivingRoom: EnumItem,
  read StoneRoom: EnumItem,
  read Auditorium: EnumItem,
  read ConcertHall: EnumItem,
  read Cave: EnumItem,
  read Arena: EnumItem,
  read Hangar: EnumItem,
  read CarpettedHallway: EnumItem,
  read Hallway: EnumItem,
  read StoneCorridor: EnumItem,
  read Alley: EnumItem,
  read Forest: EnumItem,
  read City: EnumItem,
  read Mountains: EnumItem,
  read Quarry: EnumItem,
  read Plain: EnumItem,
  read ParkingLot: EnumItem,
  read SewerPipe: EnumItem,
  read UnderWater: EnumItem,
}
type ReviewableContentState = {
  read Pending: EnumItem,
  read Completed: EnumItem,
  read Failed: EnumItem,
}
type RibbonTool = {
  read Select: EnumItem,
  read Scale: EnumItem,
  read Rotate: EnumItem,
  read Move: EnumItem,
  read Transform: EnumItem,
  read ColorPicker: EnumItem,
  read MaterialPicker: EnumItem,
  read Group: EnumItem,
  read Ungroup: EnumItem,
  read None: EnumItem,
  read PivotEditor: EnumItem,
}
type RigLabel = {
  read Invalid: EnumItem,
  read Root: EnumItem,
  read LeftHip: EnumItem,
  read LeftKnee: EnumItem,
  read LeftAnkle: EnumItem,
  read RightHip: EnumItem,
  read RightKnee: EnumItem,
  read RightAnkle: EnumItem,
  read Waist: EnumItem,
  read LeftShoulder: EnumItem,
  read LeftElbow: EnumItem,
  read LeftWrist: EnumItem,
  read RightShoulder: EnumItem,
  read RightElbow: EnumItem,
  read RightWrist: EnumItem,
  read Neck: EnumItem,
  read Spine: EnumItem,
  read Chest: EnumItem,
  read HeadBase: EnumItem,
  read LeftClavicle: EnumItem,
  read RightClavicle: EnumItem,
  read LeftToeBase: EnumItem,
  read RightToeBase: EnumItem,
  read Thumb1: EnumItem,
  read Thumb2: EnumItem,
  read Thumb3: EnumItem,
  read Index1: EnumItem,
  read Index2: EnumItem,
  read Index3: EnumItem,
  read Middle1: EnumItem,
  read Middle2: EnumItem,
  read Middle3: EnumItem,
  read Ring1: EnumItem,
  read Ring2: EnumItem,
  read Ring3: EnumItem,
  read Pinky1: EnumItem,
  read Pinky2: EnumItem,
  read Pinky3: EnumItem,
}
type RigScale = {
  read Default: EnumItem,
  read Rthro: EnumItem,
  read RthroNarrow: EnumItem,
}
type RigType = {
  read R15: EnumItem,
  read CustomHumanoid: EnumItem,
  read Custom: EnumItem,
  read None: EnumItem,
}
type RollOffMode = {
  read Inverse: EnumItem,
  read Linear: EnumItem,
  read LinearSquare: EnumItem,
  read InverseTapered: EnumItem,
}
type RolloutState = {
  read Default: EnumItem,
  read Disabled: EnumItem,
  read Enabled: EnumItem,
}
type RotationOrder = {
  read XYZ: EnumItem,
  read XZY: EnumItem,
  read YZX: EnumItem,
  read YXZ: EnumItem,
  read ZXY: EnumItem,
  read ZYX: EnumItem,
}
type RotationType = {
  read MovementRelative: EnumItem,
  read CameraRelative: EnumItem,
}
type RsvpStatus = {
  read None: EnumItem,
  read Going: EnumItem,
  read NotGoing: EnumItem,
}
type RtlTextSupport = {
  read Default: EnumItem,
  read Disabled: EnumItem,
  read Enabled: EnumItem,
}
type RunContext = {
  read Legacy: EnumItem,
  read Server: EnumItem,
  read Client: EnumItem,
  read Plugin: EnumItem,
}
type RunState = {
  read Stopped: EnumItem,
  read Running: EnumItem,
  read Paused: EnumItem,
}
type RuntimeUndoBehavior = {
  read Aggregate: EnumItem,
  read Snapshot: EnumItem,
  read Hybrid: EnumItem,
}
type SafeAreaCompatibility = {
  read None: EnumItem,
  read FullscreenExtension: EnumItem,
}
type SalesTypeFilter = {
  read All: EnumItem,
  read Collectibles: EnumItem,
  read Premium: EnumItem,
  read TimedOptions: EnumItem,
}
type SandboxedInstanceMode = {
  read Default: EnumItem,
  read Experimental: EnumItem,
}
type SaveAvatarThumbnailCustomizationFailure = {
  read BadThumbnailType: EnumItem,
  read BadYRotDeg: EnumItem,
  read BadFieldOfViewDeg: EnumItem,
  read BadDistanceScale: EnumItem,
  read Other: EnumItem,
  read Throttled: EnumItem,
}
type SaveFilter = {
  read SaveWorld: EnumItem,
  read SaveGame: EnumItem,
  read SaveAll: EnumItem,
}
type SavedQualitySetting = {
  read Automatic: EnumItem,
  read QualityLevel1: EnumItem,
  read QualityLevel2: EnumItem,
  read QualityLevel3: EnumItem,
  read QualityLevel4: EnumItem,
  read QualityLevel5: EnumItem,
  read QualityLevel6: EnumItem,
  read QualityLevel7: EnumItem,
  read QualityLevel8: EnumItem,
  read QualityLevel9: EnumItem,
  read QualityLevel10: EnumItem,
}
type ScaleType = {
  read Stretch: EnumItem,
  read Slice: EnumItem,
  read Tile: EnumItem,
  read Fit: EnumItem,
  read Crop: EnumItem,
}
type ScopeCheckResult = {
  read ConsentAccepted: EnumItem,
  read InvalidScopes: EnumItem,
  read Timeout: EnumItem,
  read NoUserInput: EnumItem,
  read BackendError: EnumItem,
  read UnexpectedError: EnumItem,
  read InvalidArgument: EnumItem,
  read ConsentDenied: EnumItem,
}
type ScreenInsets = {
  read None: EnumItem,
  read DeviceSafeInsets: EnumItem,
  read CoreUISafeInsets: EnumItem,
  read TopbarSafeInsets: EnumItem,
}
type ScreenOrientation = {
  read LandscapeLeft: EnumItem,
  read LandscapeRight: EnumItem,
  read LandscapeSensor: EnumItem,
  read Portrait: EnumItem,
  read Sensor: EnumItem,
}
type ScreenshotCaptureResult = {
  read Success: EnumItem,
  read OtherError: EnumItem,
  read NoDeviceSupport: EnumItem,
  read NoSpaceOnDevice: EnumItem,
}
type ScriptStoppedReason = {
  read Breakpoint: EnumItem,
  read Exception: EnumItem,
  read Pause: EnumItem,
  read Step: EnumItem,
  read Entry: EnumItem,
}
type ScriptVariableScope = {
  read Local: EnumItem,
  read Upvalue: EnumItem,
  read Global: EnumItem,
}
type ScrollBarInset = {
  read None: EnumItem,
  read ScrollBar: EnumItem,
  read Always: EnumItem,
}
type ScrollingDirection = {
  read X: EnumItem,
  read Y: EnumItem,
  read XY: EnumItem,
}
type SecurityCapability = {
  read RunClientScript: EnumItem,
  read RunServerScript: EnumItem,
  read AccessOutsideWrite: EnumItem,
  read AssetRequire: EnumItem,
  read LoadString: EnumItem,
  read ScriptGlobals: EnumItem,
  read CreateInstances: EnumItem,
  read Basic: EnumItem,
  read Audio: EnumItem,
  read DataStore: EnumItem,
  read Network: EnumItem,
  read Physics: EnumItem,
  read UI: EnumItem,
  read CSG: EnumItem,
  read Chat: EnumItem,
  read Animation: EnumItem,
  read Avatar: EnumItem,
  read Input: EnumItem,
  read Environment: EnumItem,
  read RemoteEvent: EnumItem,
  read LegacySound: EnumItem,
  read Players: EnumItem,
  read CapabilityControl: EnumItem,
  read Plugin: EnumItem,
  read LocalUser: EnumItem,
  read WritePlayer: EnumItem,
  read RobloxScript: EnumItem,
  read RobloxEngine: EnumItem,
  read Unassigned: EnumItem,
  read InternalTest: EnumItem,
  read PluginOrOpenCloud: EnumItem,
  read Assistant: EnumItem,
  read RemoteCommand: EnumItem,
  read AssetRead: EnumItem,
  read AssetManagement: EnumItem,
  read DynamicGeneration: EnumItem,
  read PlatformAvatarEditing: EnumItem,
  read AssetCreateUpdate: EnumItem,
  read Capture: EnumItem,
  read SensitiveInput: EnumItem,
  read Monetization: EnumItem,
  read LoadOwnedAsset: EnumItem,
  read Social: EnumItem,
  read ServerCommunication: EnumItem,
  read Logging: EnumItem,
  read PromptExternalPurchase: EnumItem,
  read Groups: EnumItem,
  read Teleport: EnumItem,
  read Consequences: EnumItem,
  read Material: EnumItem,
  read AvatarBehavior: EnumItem,
  read AvatarAppearance: EnumItem,
  read LoadUnownedAsset: EnumItem,
}
type SelectionBehavior = {
  read Escape: EnumItem,
  read Stop: EnumItem,
}
type SelectionRenderMode = {
  read Outlines: EnumItem,
  read BoundingBoxes: EnumItem,
  read Both: EnumItem,
}
type SelfViewPosition = {
  read LastPosition: EnumItem,
  read TopLeft: EnumItem,
  read TopRight: EnumItem,
  read BottomLeft: EnumItem,
  read BottomRight: EnumItem,
}
type SensorMode = {
  read Floor: EnumItem,
  read Ladder: EnumItem,
  read ClassicFloor: EnumItem,
  read ClassicLadder: EnumItem,
}
type SensorUpdateType = {
  read OnRead: EnumItem,
  read Manual: EnumItem,
}
type ServerLiveEditingMode = {
  read Uninitialized: EnumItem,
  read Enabled: EnumItem,
  read Disabled: EnumItem,
}
type ServiceVisibility = {
  read Always: EnumItem,
  read Off: EnumItem,
  read WithChildren: EnumItem,
}
type Severity = {
  read Error: EnumItem,
  read Warning: EnumItem,
  read Information: EnumItem,
  read Hint: EnumItem,
}
type ShowAdResult = {
  read ShowCompleted: EnumItem,
  read AdNotReady: EnumItem,
  read AdAlreadyShowing: EnumItem,
  read InternalError: EnumItem,
  read ShowInterrupted: EnumItem,
  read InsufficientMemory: EnumItem,
}
type SignalBehavior = {
  read Default: EnumItem,
  read Immediate: EnumItem,
  read Deferred: EnumItem,
  read AncestryDeferred: EnumItem,
}
type SimulationMode = {
  read Default: EnumItem,
  read Enabled: EnumItem,
  read Disabled: EnumItem,
}
type SizeConstraint = {
  read RelativeXY: EnumItem,
  read RelativeXX: EnumItem,
  read RelativeYY: EnumItem,
}
type SlimTintMode = {
  read None: EnumItem,
  read LOD: EnumItem,
  read Meshes: EnumItem,
  read Zone: EnumItem,
}
type SolidPrimitiveType = {
  read Capsule: EnumItem,
  read Cone: EnumItem,
  read RoundedBox: EnumItem,
}
type SolverConvergenceMetricType = {
  read IterationBased: EnumItem,
  read AlgorithmAgnostic: EnumItem,
}
type SolverConvergenceVisualizationMode = {
  read Disabled: EnumItem,
  read PerIsland: EnumItem,
  read PerEdge: EnumItem,
}
type SortDirection = {
  read Ascending: EnumItem,
  read Descending: EnumItem,
}
type SortOrder = {
  read Name: EnumItem,
  read Custom: EnumItem,
  read LayoutOrder: EnumItem,
}
type SpecialKey = {
  read Insert: EnumItem,
  read Home: EnumItem,
  read End: EnumItem,
  read PageUp: EnumItem,
  read PageDown: EnumItem,
  read ChatHotkey: EnumItem,
}
type StartCorner = {
  read TopLeft: EnumItem,
  read TopRight: EnumItem,
  read BottomLeft: EnumItem,
  read BottomRight: EnumItem,
}
type StateObjectFieldType = {
  read Boolean: EnumItem,
  read CFrame: EnumItem,
  read Color3: EnumItem,
  read Float: EnumItem,
  read Instance: EnumItem,
  read Random: EnumItem,
  read Vector2: EnumItem,
  read Vector3: EnumItem,
  read INVALID: EnumItem,
}
type Status = {
  read Poison: EnumItem,
  read Confusion: EnumItem,
}
type StepFrequency = {
  read Hz60: EnumItem,
  read Hz30: EnumItem,
  read Hz15: EnumItem,
  read Hz10: EnumItem,
  read Hz5: EnumItem,
  read Hz1: EnumItem,
}
type StreamOutBehavior = {
  read Default: EnumItem,
  read LowMemory: EnumItem,
  read Opportunistic: EnumItem,
}
type StreamingIntegrityMode = {
  read Default: EnumItem,
  read Disabled: EnumItem,
  read MinimumRadiusPause: EnumItem,
  read PauseOutsideLoadedArea: EnumItem,
}
type StreamingPauseMode = {
  read Default: EnumItem,
  read Disabled: EnumItem,
  read ClientPhysicsPause: EnumItem,
}
type StrokeSizingMode = {
  read FixedSize: EnumItem,
  read ScaledSize: EnumItem,
}
type StudioAction = {
  read Undo: EnumItem,
  read Redo: EnumItem,
  read SelectAll: EnumItem,
  read DeleteSelected: EnumItem,
  read DuplicateSelection: EnumItem,
  read ClearSelection: EnumItem,
  read ZoomExtents: EnumItem,
  read Copy: EnumItem,
  read Cut: EnumItem,
  read Paste: EnumItem,
}
type StudioCaptureBufferStatus = {
  read NotStarted: EnumItem,
  read Pending: EnumItem,
  read Ready: EnumItem,
  read Error: EnumItem,
}
type StudioCaptureScreenshotFormat = {
  read RGBA8: EnumItem,
  read PNG: EnumItem,
}
type StudioCloseMode = {
  read None: EnumItem,
  read CloseStudio: EnumItem,
  read CloseDoc: EnumItem,
  read LogOut: EnumItem,
}
type StudioDataModelType = {
  read Edit: EnumItem,
  read PlayClient: EnumItem,
  read PlayServer: EnumItem,
  read Standalone: EnumItem,
  read None: EnumItem,
}
type StudioPlaceUpdateFailureReason = {
  read Other: EnumItem,
  read TeamCreateConflict: EnumItem,
}
type StudioScriptEditorColorCategories = {
  read Default: EnumItem,
  read Operator: EnumItem,
  read Number: EnumItem,
  read String: EnumItem,
  read Comment: EnumItem,
  read Keyword: EnumItem,
  read Builtin: EnumItem,
  read Method: EnumItem,
  read Property: EnumItem,
  read Nil: EnumItem,
  read Bool: EnumItem,
  read Function: EnumItem,
  read Local: EnumItem,
  read Self: EnumItem,
  read LuauKeyword: EnumItem,
  read FunctionName: EnumItem,
  read TODO: EnumItem,
  read Background: EnumItem,
  read SelectionText: EnumItem,
  read SelectionBackground: EnumItem,
  read FindSelectionBackground: EnumItem,
  read MatchingWordBackground: EnumItem,
  read Warning: EnumItem,
  read Error: EnumItem,
  read Info: EnumItem,
  read Hint: EnumItem,
  read Whitespace: EnumItem,
  read ActiveLine: EnumItem,
  read DebuggerCurrentLine: EnumItem,
  read DebuggerErrorLine: EnumItem,
  read Ruler: EnumItem,
  read Bracket: EnumItem,
  read Type: EnumItem,
  read MenuPrimaryText: EnumItem,
  read MenuSecondaryText: EnumItem,
  read MenuSelectedText: EnumItem,
  read MenuBackground: EnumItem,
  read MenuSelectedBackground: EnumItem,
  read MenuScrollbarBackground: EnumItem,
  read MenuScrollbarHandle: EnumItem,
  read MenuBorder: EnumItem,
  read DocViewCodeBackground: EnumItem,
  read AICOOverlayText: EnumItem,
  read AICOOverlayButtonBackground: EnumItem,
  read AICOOverlayButtonBackgroundHover: EnumItem,
  read AICOOverlayButtonBackgroundPressed: EnumItem,
  read IndentationRuler: EnumItem,
}
type StudioScriptEditorColorPresets = {
  read RobloxDefault: EnumItem,
  read Extra1: EnumItem,
  read Extra2: EnumItem,
  read Custom: EnumItem,
}
type StudioStyleGuideColor = {
  read MainBackground: EnumItem,
  read Titlebar: EnumItem,
  read Dropdown: EnumItem,
  read Tooltip: EnumItem,
  read Notification: EnumItem,
  read ScrollBar: EnumItem,
  read ScrollBarBackground: EnumItem,
  read TabBar: EnumItem,
  read Tab: EnumItem,
  read FilterButtonDefault: EnumItem,
  read FilterButtonHover: EnumItem,
  read FilterButtonChecked: EnumItem,
  read FilterButtonAccent: EnumItem,
  read FilterButtonBorder: EnumItem,
  read FilterButtonBorderAlt: EnumItem,
  read RibbonTab: EnumItem,
  read RibbonTabTopBar: EnumItem,
  read Button: EnumItem,
  read MainButton: EnumItem,
  read RibbonButton: EnumItem,
  read ViewPortBackground: EnumItem,
  read InputFieldBackground: EnumItem,
  read Item: EnumItem,
  read TableItem: EnumItem,
  read CategoryItem: EnumItem,
  read GameSettingsTableItem: EnumItem,
  read GameSettingsTooltip: EnumItem,
  read EmulatorBar: EnumItem,
  read EmulatorDropDown: EnumItem,
  read ColorPickerFrame: EnumItem,
  read CurrentMarker: EnumItem,
  read Border: EnumItem,
  read DropShadow: EnumItem,
  read Shadow: EnumItem,
  read Light: EnumItem,
  read Dark: EnumItem,
  read Mid: EnumItem,
  read MainText: EnumItem,
  read SubText: EnumItem,
  read TitlebarText: EnumItem,
  read BrightText: EnumItem,
  read DimmedText: EnumItem,
  read LinkText: EnumItem,
  read WarningText: EnumItem,
  read ErrorText: EnumItem,
  read InfoText: EnumItem,
  read SensitiveText: EnumItem,
  read ScriptSideWidget: EnumItem,
  read ScriptBackground: EnumItem,
  read ScriptText: EnumItem,
  read ScriptSelectionText: EnumItem,
  read ScriptSelectionBackground: EnumItem,
  read ScriptFindSelectionBackground: EnumItem,
  read ScriptMatchingWordSelectionBackground: EnumItem,
  read ScriptOperator: EnumItem,
  read ScriptNumber: EnumItem,
  read ScriptString: EnumItem,
  read ScriptComment: EnumItem,
  read ScriptKeyword: EnumItem,
  read ScriptBuiltInFunction: EnumItem,
  read ScriptWarning: EnumItem,
  read ScriptError: EnumItem,
  read ScriptInformation: EnumItem,
  read ScriptHint: EnumItem,
  read ScriptWhitespace: EnumItem,
  read ScriptRuler: EnumItem,
  read DocViewCodeBackground: EnumItem,
  read DebuggerCurrentLine: EnumItem,
  read DebuggerErrorLine: EnumItem,
  read DiffFilePathText: EnumItem,
  read DiffTextHunkInfo: EnumItem,
  read DiffTextNoChange: EnumItem,
  read DiffTextAddition: EnumItem,
  read DiffTextDeletion: EnumItem,
  read DiffTextSeparatorBackground: EnumItem,
  read DiffTextNoChangeBackground: EnumItem,
  read DiffTextAdditionBackground: EnumItem,
  read DiffTextDeletionBackground: EnumItem,
  read DiffLineNum: EnumItem,
  read DiffLineNumSeparatorBackground: EnumItem,
  read DiffLineNumNoChangeBackground: EnumItem,
  read DiffLineNumAdditionBackground: EnumItem,
  read DiffLineNumDeletionBackground: EnumItem,
  read DiffFilePathBackground: EnumItem,
  read DiffFilePathBorder: EnumItem,
  read ChatIncomingBgColor: EnumItem,
  read ChatIncomingTextColor: EnumItem,
  read ChatOutgoingBgColor: EnumItem,
  read ChatOutgoingTextColor: EnumItem,
  read ChatModeratedMessageColor: EnumItem,
  read Separator: EnumItem,
  read ButtonBorder: EnumItem,
  read ButtonText: EnumItem,
  read InputFieldBorder: EnumItem,
  read CheckedFieldBackground: EnumItem,
  read CheckedFieldBorder: EnumItem,
  read CheckedFieldIndicator: EnumItem,
  read HeaderSection: EnumItem,
  read Midlight: EnumItem,
  read StatusBar: EnumItem,
  read DialogButton: EnumItem,
  read DialogButtonText: EnumItem,
  read DialogButtonBorder: EnumItem,
  read DialogMainButton: EnumItem,
  read DialogMainButtonText: EnumItem,
  read InfoBarWarningBackground: EnumItem,
  read InfoBarWarningText: EnumItem,
  read ScriptEditorCurrentLine: EnumItem,
  read ScriptMethod: EnumItem,
  read ScriptProperty: EnumItem,
  read ScriptNil: EnumItem,
  read ScriptBool: EnumItem,
  read ScriptFunction: EnumItem,
  read ScriptLocal: EnumItem,
  read ScriptSelf: EnumItem,
  read ScriptLuauKeyword: EnumItem,
  read ScriptFunctionName: EnumItem,
  read ScriptTodo: EnumItem,
  read ScriptBracket: EnumItem,
  read AttributeCog: EnumItem,
  read AICOOverlayText: EnumItem,
  read AICOOverlayButtonBackground: EnumItem,
  read AICOOverlayButtonBackgroundHover: EnumItem,
  read AICOOverlayButtonBackgroundPressed: EnumItem,
  read OnboardingCover: EnumItem,
  read OnboardingHighlight: EnumItem,
  read OnboardingShadow: EnumItem,
  read BreakpointMarker: EnumItem,
  read DiffLineNumHover: EnumItem,
  read DiffLineNumSeparatorBackgroundHover: EnumItem,
}
type StudioStyleGuideModifier = {
  read Default: EnumItem,
  read Selected: EnumItem,
  read Pressed: EnumItem,
  read Disabled: EnumItem,
  read Hover: EnumItem,
}
type Style = {
  read AlternatingSupports: EnumItem,
  read BridgeStyleSupports: EnumItem,
  read NoSupports: EnumItem,
}
type SubscriptionExpirationReason = {
  read ProductInactive: EnumItem,
  read ProductDeleted: EnumItem,
  read SubscriberCancelled: EnumItem,
  read SubscriberRefunded: EnumItem,
  read Lapsed: EnumItem,
}
type SubscriptionPaymentStatus = {
  read Paid: EnumItem,
  read Refunded: EnumItem,
}
type SubscriptionPeriod = {
  read Month: EnumItem,
}
type SubscriptionState = {
  read NeverSubscribed: EnumItem,
  read SubscribedWillRenew: EnumItem,
  read SubscribedWillNotRenew: EnumItem,
  read SubscribedRenewalPaymentPending: EnumItem,
  read Expired: EnumItem,
}
type SurfaceConstraint = {
  read None: EnumItem,
  read Hinge: EnumItem,
  read SteppingMotor: EnumItem,
  read Motor: EnumItem,
}
type SurfaceGuiShape = {
  read Flat: EnumItem,
  read CurvedHorizontally: EnumItem,
}
type SurfaceGuiSizingMode = {
  read FixedSize: EnumItem,
  read PixelsPerStud: EnumItem,
}
type SurfaceType = {
  read Smooth: EnumItem,
  read Glue: EnumItem,
  read Weld: EnumItem,
  read Studs: EnumItem,
  read Inlet: EnumItem,
  read Universal: EnumItem,
  read Hinge: EnumItem,
  read Motor: EnumItem,
  read SteppingMotor: EnumItem,
  read SmoothNoOutlines: EnumItem,
}
type SwipeDirection = {
  read Right: EnumItem,
  read Left: EnumItem,
  read Up: EnumItem,
  read Down: EnumItem,
  read None: EnumItem,
}
type SystemThemeValue = {
  read error: EnumItem,
  read light: EnumItem,
  read dark: EnumItem,
  read systemLight: EnumItem,
  read systemDark: EnumItem,
}
type TableMajorAxis = {
  read RowMajor: EnumItem,
  read ColumnMajor: EnumItem,
}
type TagReplicability = {}
type TeamCreateErrorState = {
  read PlaceSizeTooLarge: EnumItem,
  read PlaceSizeApproachingLimit: EnumItem,
  read PlaceUploadFailing: EnumItem,
  read NoError: EnumItem,
}
type Technology = {
  read Legacy: EnumItem,
  read Voxel: EnumItem,
  read Compatibility: EnumItem,
  read ShadowMap: EnumItem,
  read Future: EnumItem,
  read Unified: EnumItem,
}
type TelemetryBackend = {
  read UNSPECIFIED: EnumItem,
  read EventIngest: EnumItem,
  read Points: EnumItem,
  read Teletune: EnumItem,
  read EphemeralCounter: EnumItem,
  read EphemeralStat: EnumItem,
  read Counter: EnumItem,
  read Stat: EnumItem,
}
type TelemetryStandardizedField = {
  read AddDatacenterId: EnumItem,
  read AddPlaceId: EnumItem,
  read AddUniverseId: EnumItem,
  read AddPlaceInstanceId: EnumItem,
  read AddPlaySessionId: EnumItem,
  read AddCurrentContextName: EnumItem,
  read AddOsInfo: EnumItem,
  read AddArchitectureInfo: EnumItem,
  read AddCpuInfo: EnumItem,
  read AddMemoryInfo: EnumItem,
  read AddSessionInfo: EnumItem,
}
type TeleportMethod = {
  read TeleportToSpawnByName: EnumItem,
  read TeleportToPlaceInstance: EnumItem,
  read TeleportToPrivateServer: EnumItem,
  read TeleportPartyAsync: EnumItem,
  read TeleportToVIPServer: EnumItem,
  read TeleportToInstanceBack: EnumItem,
  read TeleportUnknown: EnumItem,
}
type TeleportResult = {
  read Success: EnumItem,
  read Failure: EnumItem,
  read GameNotFound: EnumItem,
  read GameEnded: EnumItem,
  read GameFull: EnumItem,
  read Unauthorized: EnumItem,
  read Flooded: EnumItem,
  read IsTeleporting: EnumItem,
}
type TeleportState = {
  read RequestedFromServer: EnumItem,
  read Started: EnumItem,
  read WaitingForServer: EnumItem,
  read Failed: EnumItem,
  read InProgress: EnumItem,
}
type TeleportType = {
  read ToPlace: EnumItem,
  read ToInstance: EnumItem,
  read ToReservedServer: EnumItem,
  read ToVIPServer: EnumItem,
  read ToInstanceBack: EnumItem,
}
type TerrainAcquisitionMethod = {
  read None: EnumItem,
  read Legacy: EnumItem,
  read Template: EnumItem,
  read Generate: EnumItem,
  read Import: EnumItem,
  read Convert: EnumItem,
  read EditAddTool: EnumItem,
  read EditSeaLevelTool: EnumItem,
  read EditReplaceTool: EnumItem,
  read RegionFillTool: EnumItem,
  read RegionPasteTool: EnumItem,
  read Other: EnumItem,
}
type TerrainFace = {
  read Top: EnumItem,
  read Side: EnumItem,
  read Bottom: EnumItem,
}
type TerrainLiquidMergeOperation = {
  read None: EnumItem,
  read Source: EnumItem,
  read Union: EnumItem,
  read Difference: EnumItem,
  read Intersect: EnumItem,
}
type TerrainSolidMergeOperation = {
  read None: EnumItem,
  read Paint: EnumItem,
  read Source: EnumItem,
  read Union: EnumItem,
  read Dig: EnumItem,
  read Difference: EnumItem,
  read Intersect: EnumItem,
  read Cut: EnumItem,
  read Place: EnumItem,
}
type TextChatMessageStatus = {
  read Unknown: EnumItem,
  read Success: EnumItem,
  read Sending: EnumItem,
  read TextFilterFailed: EnumItem,
  read Floodchecked: EnumItem,
  read InvalidPrivacySettings: EnumItem,
  read InvalidTextChannelPermissions: EnumItem,
  read MessageTooLong: EnumItem,
  read ModerationTimeout: EnumItem,
}
type TextDirection = {
  read Auto: EnumItem,
  read LeftToRight: EnumItem,
  read RightToLeft: EnumItem,
}
type TextFilterContext = {
  read PublicChat: EnumItem,
  read PrivateChat: EnumItem,
}
type TextInputType = {
  read Default: EnumItem,
  read NoSuggestions: EnumItem,
  read Number: EnumItem,
  read Email: EnumItem,
  read Phone: EnumItem,
  read Password: EnumItem,
  read PasswordShown: EnumItem,
  read Username: EnumItem,
  read OneTimePassword: EnumItem,
  read NewPassword: EnumItem,
  read NewPasswordShown: EnumItem,
}
type TextTruncate = {
  read None: EnumItem,
  read AtEnd: EnumItem,
  read SplitWord: EnumItem,
}
type TextXAlignment = {
  read Left: EnumItem,
  read Right: EnumItem,
  read Center: EnumItem,
}
type TextYAlignment = {
  read Top: EnumItem,
  read Center: EnumItem,
  read Bottom: EnumItem,
}
type TextureMode = {
  read Stretch: EnumItem,
  read Wrap: EnumItem,
  read Static: EnumItem,
}
type TextureQueryType = {
  read NonHumanoid: EnumItem,
  read NonHumanoidOrphaned: EnumItem,
  read Humanoid: EnumItem,
  read HumanoidOrphaned: EnumItem,
}
type ThreadPoolConfig = {
  read Auto: EnumItem,
  read Threads1: EnumItem,
  read Threads2: EnumItem,
  read Threads3: EnumItem,
  read Threads4: EnumItem,
  read Threads8: EnumItem,
  read Threads16: EnumItem,
  read PerCore1: EnumItem,
  read PerCore2: EnumItem,
  read PerCore3: EnumItem,
  read PerCore4: EnumItem,
}
type ThrottlingPriority = {
  read Default: EnumItem,
  read ElevatedOnServer: EnumItem,
  read Extreme: EnumItem,
}
type ThumbnailSize = {
  read Size48x48: EnumItem,
  read Size180x180: EnumItem,
  read Size420x420: EnumItem,
  read Size60x60: EnumItem,
  read Size100x100: EnumItem,
  read Size150x150: EnumItem,
  read Size352x352: EnumItem,
}
type ThumbnailType = {
  read HeadShot: EnumItem,
  read AvatarBust: EnumItem,
  read AvatarThumbnail: EnumItem,
}
type TickCountSampleMethod = {
  read Fast: EnumItem,
  read Benchmark: EnumItem,
  read Precise: EnumItem,
}
type TonemapperPreset = {
  read Default: EnumItem,
  read Retro: EnumItem,
}
type TopBottom = {
  read Top: EnumItem,
  read Center: EnumItem,
  read Bottom: EnumItem,
}
type TouchCameraMovementMode = {
  read Default: EnumItem,
  read Classic: EnumItem,
  read Follow: EnumItem,
  read Orbital: EnumItem,
}
type TouchMovementMode = {
  read Default: EnumItem,
  read Thumbstick: EnumItem,
  read DPad: EnumItem,
  read Thumbpad: EnumItem,
  read ClickToMove: EnumItem,
  read DynamicThumbstick: EnumItem,
}
type TrackerError = {
  read Ok: EnumItem,
  read NoService: EnumItem,
  read InitFailed: EnumItem,
  read NoVideo: EnumItem,
  read VideoError: EnumItem,
  read VideoNoPermission: EnumItem,
  read VideoUnsupported: EnumItem,
  read NoAudio: EnumItem,
  read AudioError: EnumItem,
  read AudioNoPermission: EnumItem,
  read UnsupportedDevice: EnumItem,
}
type TrackerExtrapolationFlagMode = {
  read ForceDisabled: EnumItem,
  read ExtrapolateFacsAndPose: EnumItem,
  read ExtrapolateFacsOnly: EnumItem,
  read Auto: EnumItem,
}
type TrackerFaceTrackingStatus = {
  read FaceTrackingSuccess: EnumItem,
  read FaceTrackingNoFaceFound: EnumItem,
  read FaceTrackingUnknown: EnumItem,
  read FaceTrackingLost: EnumItem,
  read FaceTrackingHasTrackingError: EnumItem,
  read FaceTrackingIsOccluded: EnumItem,
  read FaceTrackingUninitialized: EnumItem,
}
type TrackerLodFlagMode = {
  read ForceFalse: EnumItem,
  read ForceTrue: EnumItem,
  read Auto: EnumItem,
}
type TrackerLodValueMode = {
  read Force0: EnumItem,
  read Force1: EnumItem,
  read Auto: EnumItem,
}
type TrackerMode = {
  read None: EnumItem,
  read Audio: EnumItem,
  read Video: EnumItem,
  read AudioVideo: EnumItem,
}
type TrackerPromptEvent = {
  read LODCameraRecommendDisable: EnumItem,
}
type TrackerType = {
  read None: EnumItem,
  read Face: EnumItem,
  read UpperBody: EnumItem,
}
type TriStateBoolean = {
  read Unknown: EnumItem,
  read True: EnumItem,
  read False: EnumItem,
}
type TweenStatus = {
  read Canceled: EnumItem,
  read Completed: EnumItem,
}
type UICaptureMode = {
  read All: EnumItem,
  read None: EnumItem,
}
type UIDragDetectorBoundingBehavior = {
  read Automatic: EnumItem,
  read EntireObject: EnumItem,
  read HitPoint: EnumItem,
}
type UIDragDetectorDragRelativity = {
  read Absolute: EnumItem,
  read Relative: EnumItem,
}
type UIDragDetectorDragSpace = {
  read Parent: EnumItem,
  read LayerCollector: EnumItem,
  read Reference: EnumItem,
}
type UIDragDetectorDragStyle = {
  read TranslatePlane: EnumItem,
  read TranslateLine: EnumItem,
  read Rotate: EnumItem,
  read Scriptable: EnumItem,
}
type UIDragDetectorResponseStyle = {
  read Offset: EnumItem,
  read Scale: EnumItem,
  read CustomOffset: EnumItem,
  read CustomScale: EnumItem,
}
type UIDragSpeedAxisMapping = {
  read XY: EnumItem,
  read XX: EnumItem,
  read YY: EnumItem,
}
type UIFlexAlignment = {
  read None: EnumItem,
  read Fill: EnumItem,
  read SpaceAround: EnumItem,
  read SpaceBetween: EnumItem,
  read SpaceEvenly: EnumItem,
}
type UIFlexMode = {
  read None: EnumItem,
  read Grow: EnumItem,
  read Shrink: EnumItem,
  read Fill: EnumItem,
  read Custom: EnumItem,
}
type UITheme = {
  read Light: EnumItem,
  read Dark: EnumItem,
}
type UiMessageType = {
  read UiMessageError: EnumItem,
  read UiMessageInfo: EnumItem,
}
type UpdateState = {
  read UpdateNotAvailable: EnumItem,
  read UpdateAvailable: EnumItem,
  read UpdateInProgress: EnumItem,
  read UpdateReady: EnumItem,
  read UpdateFailed: EnumItem,
}
type UploadCaptureResult = {
  read Success: EnumItem,
  read NeedPermission: EnumItem,
  read CaptureModerated: EnumItem,
  read CaptureNotInGallery: EnumItem,
  read IneligibleCapture: EnumItem,
  read UploadQuotaReached: EnumItem,
  read UploadPending: EnumItem,
  read UploadFailed: EnumItem,
}
type UsageContext = {
  read Default: EnumItem,
  read Preview: EnumItem,
}
type UserCFrame = {
  read Head: EnumItem,
  read LeftHand: EnumItem,
  read RightHand: EnumItem,
  read Floor: EnumItem,
}
type UserInputState = {
  read Begin: EnumItem,
  read Change: EnumItem,
  read End: EnumItem,
  read Cancel: EnumItem,
  read None: EnumItem,
}
type UserInputType = {
  read MouseButton1: EnumItem,
  read MouseButton2: EnumItem,
  read MouseButton3: EnumItem,
  read MouseWheel: EnumItem,
  read MouseMovement: EnumItem,
  read Touch: EnumItem,
  read Keyboard: EnumItem,
  read Focus: EnumItem,
  read Accelerometer: EnumItem,
  read Gyro: EnumItem,
  read Gamepad1: EnumItem,
  read Gamepad2: EnumItem,
  read Gamepad3: EnumItem,
  read Gamepad4: EnumItem,
  read Gamepad5: EnumItem,
  read Gamepad6: EnumItem,
  read Gamepad7: EnumItem,
  read Gamepad8: EnumItem,
  read TextInput: EnumItem,
  read InputMethod: EnumItem,
  read None: EnumItem,
}
type VRComfortSetting = {
  read Comfort: EnumItem,
  read Normal: EnumItem,
  read Expert: EnumItem,
  read Custom: EnumItem,
}
type VRControllerModelMode = {
  read Disabled: EnumItem,
  read Transparent: EnumItem,
}
type VRDeviceType = {
  read Unknown: EnumItem,
  read OculusRift: EnumItem,
  read HTCVive: EnumItem,
  read ValveIndex: EnumItem,
  read OculusQuest: EnumItem,
}
type VRLaserPointerMode = {
  read Disabled: EnumItem,
  read Pointer: EnumItem,
  read DualPointer: EnumItem,
}
type VRSafetyBubbleMode = {
  read NoOne: EnumItem,
  read OnlyFriends: EnumItem,
  read Anyone: EnumItem,
}
type VRScaling = {
  read World: EnumItem,
  read Off: EnumItem,
}
type VRSessionState = {
  read Undefined: EnumItem,
  read Idle: EnumItem,
  read Visible: EnumItem,
  read Focused: EnumItem,
  read Stopping: EnumItem,
}
type VRTouchpad = {
  read Left: EnumItem,
  read Right: EnumItem,
}
type VRTouchpadMode = {
  read Touch: EnumItem,
  read VirtualThumbstick: EnumItem,
  read ABXY: EnumItem,
}
type VelocityConstraintMode = {
  read Line: EnumItem,
  read Plane: EnumItem,
  read Vector: EnumItem,
}
type VerticalAlignment = {
  read Center: EnumItem,
  read Top: EnumItem,
  read Bottom: EnumItem,
}
type VerticalScrollBarPosition = {
  read Right: EnumItem,
  read Left: EnumItem,
}
type VibrationMotor = {
  read Large: EnumItem,
  read Small: EnumItem,
  read LeftTrigger: EnumItem,
  read RightTrigger: EnumItem,
  read LeftHand: EnumItem,
  read RightHand: EnumItem,
}
type VideoCaptureResult = {
  read Success: EnumItem,
  read OtherError: EnumItem,
  read ScreenSizeChanged: EnumItem,
  read TimeLimitReached: EnumItem,
}
type VideoCaptureStartedResult = {
  read Success: EnumItem,
  read OtherError: EnumItem,
  read CapturingAlready: EnumItem,
  read NoDeviceSupport: EnumItem,
  read NoSpaceOnDevice: EnumItem,
}
type VideoDeviceCaptureQuality = {
  read Default: EnumItem,
  read Low: EnumItem,
  read Medium: EnumItem,
  read High: EnumItem,
}
type VideoError = {
  read Ok: EnumItem,
  read Eof: EnumItem,
  read EAgain: EnumItem,
  read BadParameter: EnumItem,
  read AllocFailed: EnumItem,
  read CodecInitFailed: EnumItem,
  read CodecCloseFailed: EnumItem,
  read DecodeFailed: EnumItem,
  read ParsingFailed: EnumItem,
  read Unsupported: EnumItem,
  read Generic: EnumItem,
  read DownloadFailed: EnumItem,
  read StreamNotFound: EnumItem,
  read EncodeFailed: EnumItem,
  read CreateFailed: EnumItem,
  read NoPermission: EnumItem,
  read NoService: EnumItem,
  read ReleaseFailed: EnumItem,
  read Unknown: EnumItem,
}
type VideoSampleSize = {
  read Small: EnumItem,
  read Medium: EnumItem,
  read Large: EnumItem,
  read Full: EnumItem,
}
type ViewMode = {
  read None: EnumItem,
  read GeometryComplexity: EnumItem,
  read Transparent: EnumItem,
  read Decal: EnumItem,
}
type VirtualCursorMode = {
  read Default: EnumItem,
  read Disabled: EnumItem,
  read Enabled: EnumItem,
}
type VirtualInputMode = {
  read None: EnumItem,
  read Recording: EnumItem,
  read Playing: EnumItem,
}
type VoiceChatDistanceAttenuationType = {
  read Inverse: EnumItem,
  read Legacy: EnumItem,
}
type VoiceChatState = {
  read Idle: EnumItem,
  read Joining: EnumItem,
  read JoiningRetry: EnumItem,
  read Joined: EnumItem,
  read Leaving: EnumItem,
  read Ended: EnumItem,
  read Failed: EnumItem,
}
type VoiceClientLeaveReasons = {
  read Unknown: EnumItem,
  read ClientNetworkDisconnected: EnumItem,
  read PlayerLeft: EnumItem,
  read ClientShutdown: EnumItem,
  read PublishFailed: EnumItem,
  read RejoinReceived: EnumItem,
  read VoiceReboot: EnumItem,
  read ImguiDebugLeave: EnumItem,
  read LuaInitiated: EnumItem,
}
type VoiceControlPath = {
  read Publish: EnumItem,
  read Subscribe: EnumItem,
  read Join: EnumItem,
}
type VoiceRccReconnectReason = {
  read Unknown: EnumItem,
  read Migration: EnumItem,
  read CloseRoom: EnumItem,
  read FAEUpdate: EnumItem,
  read BlockListChanged: EnumItem,
}
type VolumetricAudio = {
  read Disabled: EnumItem,
  read Automatic: EnumItem,
  read Enabled: EnumItem,
}
type WaterDirection = {
  read NegX: EnumItem,
  read X: EnumItem,
  read NegY: EnumItem,
  read Y: EnumItem,
  read NegZ: EnumItem,
  read Z: EnumItem,
}
type WaterForce = {
  read None: EnumItem,
  read Small: EnumItem,
  read Medium: EnumItem,
  read Strong: EnumItem,
  read Max: EnumItem,
}
type WebSocketState = {
  read Connecting: EnumItem,
  read Open: EnumItem,
  read Closing: EnumItem,
  read Closed: EnumItem,
}
type WebStreamClientState = {
  read Connecting: EnumItem,
  read Open: EnumItem,
  read Error: EnumItem,
  read Closed: EnumItem,
}
type WebStreamClientType = {
  read SSE: EnumItem,
  read RawStream: EnumItem,
  read WebSocket: EnumItem,
}
type WeldConstraintPreserve = {
  read All: EnumItem,
  read None: EnumItem,
  read Touching: EnumItem,
}
type WhenUserFirstPlayed = {
  read Unknown: EnumItem,
  read Days0To30: EnumItem,
  read Days31To90: EnumItem,
  read Days91To180: EnumItem,
  read Days181To365: EnumItem,
  read Days366Plus: EnumItem,
}
type WhisperChatPrivacyMode = {
  read AllUsers: EnumItem,
  read NoOne: EnumItem,
}
type WrapLayerAutoSkin = {
  read Disabled: EnumItem,
  read EnabledPreserve: EnumItem,
  read EnabledOverride: EnumItem,
}
type WrapLayerDebugMode = {
  read None: EnumItem,
  read BoundCage: EnumItem,
  read LayerCage: EnumItem,
  read BoundCageAndLinks: EnumItem,
  read Reference: EnumItem,
  read Rbf: EnumItem,
  read OuterCage: EnumItem,
  read ReferenceMeshAfterMorph: EnumItem,
  read HSROuterDetail: EnumItem,
  read HSROuter: EnumItem,
  read HSRInner: EnumItem,
  read HSRInnerReverse: EnumItem,
  read LayerCageFittedToBase: EnumItem,
  read LayerCageFittedToPrev: EnumItem,
  read PreWrapDeformerOuterCage: EnumItem,
  read SkinningTransfer: EnumItem,
}
type WrapTargetDebugMode = {
  read None: EnumItem,
  read TargetCageOriginal: EnumItem,
  read TargetCageCompressed: EnumItem,
  read TargetCageInterface: EnumItem,
  read TargetLayerCageOriginal: EnumItem,
  read TargetLayerCageCompressed: EnumItem,
  read TargetLayerInterface: EnumItem,
  read Rbf: EnumItem,
  read OuterCageDetail: EnumItem,
  read PreWrapDeformerCage: EnumItem,
}
type ZIndexBehavior = {
  read Global: EnumItem,
  read Sibling: EnumItem,
}
type Enum = {
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
const water = Enum.Material.Water
print(water)
print(water.Name)
print(water.Value)
