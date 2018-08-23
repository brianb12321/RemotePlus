namespace RemotePlusServer.UnitTests
{
    public class MockConfigAccess : RemotePlusLibrary.Configuration.IConfigurationDataAccess
    {
        public TConfigModel LoadConfig<TConfigModel>(string file)
        {
            TConfigModel model = default(TConfigModel);
            return model;
        }

        public void SaveConfig<TConfigModel>(TConfigModel configType, string file)
        {
            
        }
    }
}