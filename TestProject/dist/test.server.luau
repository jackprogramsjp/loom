-- Handwritten Roblox types
--declare fn print(msg: unknown): void;
-- Object: Object?;
type SecurityCapabilities = {}
type Instance = {}
type DateTime = {
  read UnixTimestamp: number,
  read UnixTimestampMillis: number,
}
type DateTimeStatic = {
  read now: () -> DateTime,
  read fromIsoDate: (string) -> DateTime?,
  read fromLocalTime: (number?, number?, number?, number?, number?, number?, number?) -> DateTime,
  read fromUniversalTime: (number?, number?, number?, number?, number?, number?, number?) -> DateTime,
  read fromUnixTimestamp: (number) -> DateTime,
  read fromUnixTimestampMillis: (number) -> DateTime,
}
type Content = {
  read SourceType: index<Enum, "ContentSourceType">,
  read Uri: string?,
}
type EnumItem<Kind> = {
  read _kind: Kind & string,
  read Name: string,
  read Value: number,
  read EnumType: never,
}
type EnumAccessModifierType = {
  read Allow: EnumItem<"AccessModifierType">,
  read Deny: EnumItem<"AccessModifierType">,
}
type Enum = {
  read AccessModifierType: EnumItem<"AccessModifierType">,
  read ContentSourceType: EnumItem<"ContentSourceType">,
}
