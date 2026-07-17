type Instance = {
  Archivable: boolean,
  Name: string,
  Parent: Instance?,
  Sandboxed: boolean,
}
type ServerStorage = Instance & {
  read Poo: number,
}
type Services = {
  read ServerStorage: ServerStorage,
}
const _ = get_service("ServerStorage").Poo + 69
