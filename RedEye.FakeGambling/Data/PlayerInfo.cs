using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RedEye.FakeGambling.Data
{
    public class PlayerInfo
    {
        [Required]
        public decimal BetAmount { get; set; }
        [Required]
        public decimal AutoCashOut { get; set; }
        public decimal PlayerBal { get; set; }
    }
}
