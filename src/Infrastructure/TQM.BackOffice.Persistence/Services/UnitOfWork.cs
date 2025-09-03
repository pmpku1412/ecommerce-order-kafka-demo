using Microsoft.Extensions.Configuration;

namespace TQM.BackOffice.Persistence.Services;

public class UnitOfWork : IUnitOfWork
{

    private readonly IConfiguration _configuration;
    private readonly IDBAdapter _dbAdapter;

    //private IReceiveService? _receiveService;
    private IMailServiceX _mailService;

    public UnitOfWork(IConfiguration configuration, IDBAdapter dbAdapter,  IMailServiceX mailService)
    {
        _configuration = configuration;
        _dbAdapter = dbAdapter;
        _mailService = mailService;
    }

    //ISaleService IUnitOfWork.SaleService
    //{
    //    get
    //    {
    //        return _saleService ??= new SaleService(_dbAdapter, _productService, _configuration);
    //    }
    //}

    //public IReceiveService ReceiveService => _receiveService ??= new ReceiveService(_configuration, _dbAdapter, _mailService);

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
