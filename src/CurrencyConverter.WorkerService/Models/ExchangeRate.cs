using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConverter.WorkerService.Models
{
	internal class ExchangeRate
	{
		public int Id { get; set; }
		public int SnapshotId { get; set; }
		public string CurrencyCode { get; set; }
		public decimal Rate { get; set; }
		public ExchangeRateSnapshot Snapshot { get; set; }
	}
}
