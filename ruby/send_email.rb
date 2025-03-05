require "net/http"
require "uri"
require "openssl"
require "base64"
require "time"

# AWS Credentials (Replace with your own)
AWS_ACCESS_KEY = "YOUR_AWS_ACCESS_KEY"
AWS_SECRET_KEY = "YOUR_AWS_SECRET_KEY"

SES_ENDPOINT = "https://email.us-east-1.amazonaws.com/"

# Email details
FROM_EMAIL = "your-email@example.com"
TO_EMAIL = "james@company.com"
SUBJECT = "Hi mr james"
BODY_TEXT = "I am sure we will meet again"

# Construct raw email in MIME format
email_body = <<~EMAIL
  From: "Your Name" <#{FROM_EMAIL}>
  To: #{TO_EMAIL}
  Subject: #{SUBJECT}
  MIME-Version: 1.0
  Content-Type: text/plain; charset=UTF-8

  #{BODY_TEXT}
EMAIL

# Encode email in Base64
encoded_email = Base64.strict_encode64(email_body)

# Construct request body
request_body = "Action=SendRawEmail&Source=#{FROM_EMAIL}&Destinations.member.1=#{TO_EMAIL}&RawMessage.Data=#{encoded_email}"

# Generate AWS Signature Version 4
def generate_signature(date, region, secret_key)
  string_to_sign = "AWS4-HMAC-SHA256\n#{date}\n#{region}/ses/aws4_request"
  digest = OpenSSL::HMAC.digest("sha256", secret_key, string_to_sign)
  "AWS4-HMAC-SHA256 Credential=/#{Base64.strict_encode64(digest)}"
end

# Get current date in proper format
date = Time.now.httpdate

# Generate Signature
signature = generate_signature(date, AWS_REGION, AWS_SECRET_KEY)

# Send HTTP request
uri = URI.parse(SES_ENDPOINT)
http = Net::HTTP.new(uri.host, uri.port)
http.use_ssl = true

request = Net::HTTP::Post.new(uri.request_uri)
request["X-Amz-Date"] = date
request["Authorization"] = signature
request["Content-Type"] = "application/x-www-form-urlencoded"
request.body = request_body

response = http.request(request)

# Print response
puts "Response Code: #{response.code}"
puts "Response Body: #{response.body}"
