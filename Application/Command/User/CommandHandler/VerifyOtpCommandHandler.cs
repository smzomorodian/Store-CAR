using Application.Command.User.Command;
using Infrustructure.Repository.IRepository;
using MediatR;
using Usermodel = Domain.Model.User;

public class VerifyOtpCommandHandler<T> : IRequestHandler<VerifyOtpCommand<T>, string> where T : Usermodel
{
    private readonly IRepository<T> _genericRepository;
    private readonly IUserInfoRepository<T> _userInfoRepository;

    public VerifyOtpCommandHandler(IRepository<T> genericRepository, IUserInfoRepository<T> userInfoRepository)
    {
        _genericRepository = genericRepository;
        _userInfoRepository = userInfoRepository;
    }

    public async Task<string> Handle(VerifyOtpCommand<T> request, CancellationToken cancellationToken)
    {
        var user = await _userInfoRepository.getphonenmber(request.PhoneNumber);

        if (user == null)
            return "کاربر یافت نشد";

        if (user.Otp == null || user.OtpExpiry < DateTime.UtcNow)
            return "OTP نامعتبر یا منقضی شده است";

        if (user.Otp != request.otp)
            return "OTP اشتباه است";

        user.Otp = null;
        user.OtpExpiry = null;

        await _genericRepository.SavechangeAsync();

        return "ورود موفقیت‌آمیز بود";
    }
}
