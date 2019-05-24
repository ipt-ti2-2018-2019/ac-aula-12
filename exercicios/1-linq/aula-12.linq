<Query Kind="Expression">
  <Connection>
    <ID>5230142b-67d6-4a8d-8dca-5e96fc95e5bb</ID>
    <Persist>true</Persist>
    <Server>(localdb)\MSSQLLocalDB</Server>
    <AttachFile>true</AttachFile>
    <AttachFileName>C:\Users\0100975\source\repos\ipt\ti2\ac-aula-12\exercicios\1-linq\MultasDB.mdf</AttachFileName>
  </Connection>
</Query>

Multas
	.Skip(15)
	.Take(5)

//Agentes
//	.Where(a => a.Esquadra == "Ourém")
//	.Any()

//Agentes.Any(a => a.Esquadra == "Ourém")

//Agentes
//	.Where(a => a.Esquadra == "Ourém")
//	.Count()

//Agentes.Count(a => a.Esquadra == "Ourém")

//Agentes
//	.Join(
//		Multas,
//		a => a.ID,
//		m => m.AgenteFK,
//		(agente, multa) => new {
//			NomeAgente = agente.Nome,
//			LocalMulta = multa.LocalDaMulta,
//			ValorMulta = multa.ValorMulta
//		})

//Multas
//	.GroupBy(
//		m => m.LocalDaMulta,
//		(localDaMulta, multas) => new {
//			Local = localDaMulta,
//			NumMultas = multas.Count(),
//			Total = multas.Sum(m => m.ValorMulta)
//		}
//	)
//	.OrderByDescending(m => m.NumMultas)

//Multas
//	.OrderByDescending(m => m.ValorMulta)
//	.ThenBy(m => m.DataDaMulta)

//from a in Agentes
//select new {
//	Nome = a.Nome,
//	Esquadra = a.Esquadra
//}

//Agentes.Select(a => new {
//	Nome = a.Nome,
//	Esquadra = a.Esquadra
//})



//from n in new[] { 1,2,3,4,5}
//select n * 2

//new[] { 1,2,3,4,5}
//	.Select(n => n * 2)






//from a in Agentes
//where a.Esquadra == "Ourém"
//select a

//Agentes.Where(a => a.Esquadra == "Ourém")