namespace AuthNet.Models.DTO
{
    public class SalesForecast
    {
        [Microsoft.ML.Data.VectorType]
        public float[] Forecasted { get; set; }

        [Microsoft.ML.Data.VectorType]
        public float[] LowerBound { get; set; }

        [Microsoft.ML.Data.VectorType]
        public float[] UpperBound { get; set; }
    }

    public class SalesPoint
    {
        public float Quantity { get; set; }   // per day
    }

    public class DailySaleData
    {
        public float Quantity { get; set; } 
    }

    public class DailySaleForecast
    {
        public float[] Forecasted { get; set; }   // predicted values (multiple days)
    }

}

