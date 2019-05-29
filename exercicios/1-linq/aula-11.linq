<Query Kind="Expression">
  <Connection>
    <ID>928ac3d2-d761-44fc-9c53-567645796d7d</ID>
    <Persist>true</Persist>
    <Server>(localdb)\MSSQLLocalDB</Server>
    <AttachFile>true</AttachFile>
    <AttachFileName>C:\Users\0100975\source\repos\ipt\ti2\ac-aula-12\exercicios\1-linq\MultasDB.mdf</AttachFileName>
  </Connection>
</Query>

Agentes
	.Join(
		Multas,
		// Chave dos Agentes
		agente => agente.ID,
		// Chave das Multas
		multa => multa.AgenteFK,
		(agente, multa) => new {
			agente.Nome,
			multa.Infracao,
			multa.LocalDaMulta,
			multa.ValorMulta
		}
	)


//Multas
//	.GroupBy(
//		m => m.LocalDaMulta,
//		(local, multasDoLocal) => new {
//			Local = local,
//			NumMultas = multasDoLocal.Count(),
//			Total = multasDoLocal.Sum(m => m.ValorMulta)
//		}
//	)
//	.OrderByDescending(m => m.Total)

//Multas
//	.Select(m => new {
//		m.Infracao,
//		m.LocalDaMulta,
//		m.ValorMulta
//	})
//	.OrderBy(m => m.LocalDaMulta)
//	.ThenByDescending(m => m.ValorMulta)

//Agentes.Where(a => a.Esquadra == "Ourém")

// METHOD SYNTAX

//new[] { 1, 2, 3, 4, 5 }
//	.Where(n => n > 1)
//	.Select(n => n * 2)

// QUERY SYNTAX

//from n in new[] { 1, 2, 3, 4, 5 }
//where n > 1
//select n * 2

//from n in new[] { 1, 2, 3, 4, 5 }
//select n * 2