namespace Service.DTO
{
    public static class EmailBodyTemplate
    {
        public static string GetRegistrationConfirmationEmail(string username, string confirmationLink)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: 'Helvetica Neue', Arial, sans-serif; margin: 0; padding: 0; background-color: #f7f7f7; color: #333; }}
        .container {{ max-width: 650px; margin: 40px auto; background-color: #ffffff; border: 1px solid #e0e0e0; border-radius: 8px; overflow: hidden; }}
        .header {{ background-color: #003087; color: #ffffff; padding: 20px; text-align: center; }}
        .header h1 {{ margin: 0; font-size: 24px; font-weight: 600; }}
        .content {{ padding: 30px; text-align: left; line-height: 1.6; }}
        .content p {{ margin: 0 0 15px; }}
        .button {{ display: inline-block; padding: 12px 25px; background-color: #003087; color: #ffffff; text-decoration: none; border-radius: 5px; font-size: 16px; font-weight: 500; transition: background-color 0.3s; }}
        .button:hover {{ background-color: #00266b; }}
        .footer {{ background-color: #f7f7f7; padding: 20px; text-align: center; font-size: 12px; color: #777; border-top: 1px solid #e0e0e0; }}
        .footer a {{ color: #003087; text-decoration: none; }}
        .footer a:hover {{ text-decoration: underline; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>FlashWork</h1>
        </div>
        <div class='content'>
            <p>Dear {username},</p>
            <p>Thank you for registering with FlashWork. To complete the activation of your account and ensure secure access, please confirm your email address by clicking the button below.</p>
            <p><a href='{confirmationLink}' class='button'>Confirm Your Email</a></p>
            <p>If you are unable to click the button, please copy and paste the following link into your browser:</p>
            <p><a href='{confirmationLink}'>{confirmationLink}</a></p>
            <p>Should you have any questions or require assistance, please do not hesitate to contact our support team.</p>
        </div>
        <div class='footer'>
            <p>© {DateTime.UtcNow.Year} FlashWork. All rights reserved. | <a href='mailto:support@flashwork.com'>support@flashwork.com</a> | +1-800-555-1234</p>
            <p>This email contains confidential information and is intended solely for the use of the individual to whom it is addressed. If you are not the intended recipient, please notify the sender and delete this email.</p>
        </div>
    </div>
</body>
</html>";
        }

        public static string GetPasswordResetEmail(string username, string resetLink)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: 'Helvetica Neue', Arial, sans-serif; margin: 0; padding: 0; background-color: #f7f7f7; color: #333; }}
        .container {{ max-width: 650px; margin: 40px auto; background-color: #ffffff; border: 1px solid #e0e0e0; border-radius: 8px; overflow: hidden; }}
        .header {{ background-color: #003087; color: #ffffff; padding: 20px; text-align: center; }}
        .header h1 {{ margin: 0; font-size: 24px; font-weight: 600; }}
        .content {{ padding: 30px; text-align: left; line-height: 1.6; }}
        .content p {{ margin: 0 0 15px; }}
        .button {{ display: inline-block; padding: 12px 25px; background-color: #003087; color: #ffffff; text-decoration: none; border-radius: 5px; font-size: 16px; font-weight: 500; transition: background-color 0.3s; }}
        .button:hover {{ background-color: #00266b; }}
        .footer {{ background-color: #f7f7f7; padding: 20px; text-align: center; font-size: 12px; color: #777; border-top: 1px solid #e0e0e0; }}
        .footer a {{ color: #003087; text-decoration: none; }}
        .footer a:hover {{ text-decoration: underline; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>FlashWork</h1>
        </div>
        <div class='content'>
            <p>Dear {username},</p>
            <p>We have received a request to reset the password for your FlashWork account. To proceed, please click the button below to reset your password.</p>
            <p><a href='{resetLink}' class='button'>Reset Your Password</a></p>
            <p>If you are unable to click the button, please copy and paste the following link into your browser:</p>
            <p><a href='{resetLink}'>{resetLink}</a></p>
            <p>If you did not initiate this request, please disregard this email or contact our support team immediately to secure your account.</p>
        </div>
        <div class='footer'>
            <p>© {DateTime.UtcNow.Year} FlashWork. All rights reserved. | <a href='mailto:support@flashwork.com'>support@flashwork.com</a> | +1-800-555-1234</p>
            <p>This email contains confidential information and is intended solely for the use of the individual to whom it is addressed. If you are not the intended recipient, please notify the sender and delete this email.</p>
        </div>
    </div>
</body>
</html>";
        }
    }
}