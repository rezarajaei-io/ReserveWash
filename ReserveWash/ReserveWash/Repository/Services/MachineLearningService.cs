using Microsoft.ML;
using ReserveWash.AI;
using System.Globalization;

namespace ReserveWash.Repository.Services
{
    public class MachineLearningService
    {
        private readonly MLContext _mlContext;

        public MachineLearningService()
        {
            _mlContext = new MLContext();
        }
        public void TrainModel(IEnumerable<CustomerServiceData> data)
        {
            // تبدیل داده‌ها به IDataView
            IDataView trainingData = _mlContext.Data.LoadFromEnumerable(data);

            // تعریف فرآیند آموزشی: اینجا از رگرسیون خطی استفاده می‌کنیم
            var pipeline = _mlContext.Regression.Trainers.Sdca(labelColumnName: "ServiceCost",
                                                               featureColumnName: "Features");

            // آموزش مدل
            var trainedModel = pipeline.Fit(trainingData);

            // ذخیره مدل به فایل
            _mlContext.Model.Save(trainedModel, trainingData.Schema, "trained_model.zip");
        }

        public ServicePrediction PredictServiceCost(CustomerServiceData inputData)
        {
            // بارگذاری داده‌ها و مدل
            var model = LoadModel();

            // ایجاد PredictionEngine
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<CustomerServiceData, ServicePrediction>(model);

            // انجام پیش‌بینی
            var prediction = predictionEngine.Predict(inputData);

            return prediction;
        }

        private ITransformer LoadModel()
        {
            // بارگذاری مدل از فایل
            return _mlContext.Model.Load("path_to_trained_model.zip", out var modelInputSchema);
        }
    }

}
