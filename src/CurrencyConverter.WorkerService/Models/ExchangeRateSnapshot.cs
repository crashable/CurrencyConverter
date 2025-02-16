using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConverter.WorkerService.Models
{
	internal class ExchangeRateSnapshot
	{
		public int Id { get; set; }
		public DateTime SnapshotDate { get; set; }
		public string BaseCurrency { get; set; }
		public long Timestamp { get; set; }
		public ICollection<ExchangeRate> Rates { get; set; }
	}
}
