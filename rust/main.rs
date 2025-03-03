use reqwest::blocking::Client;
use chrono::Utc;
use hmac::{Hmac, Mac};
use sha2::Sha256;
use base64::{engine::general_purpose, Engine as _};
use std::env;

type HmacSha256 = Hmac<Sha256>;

fn main() {
    let access_key = "YOUR_AWS_ACCESS_KEY";
    let secret_key = "YOUR_AWS_SECRET_KEY";
    let ses_endpoint = "https://email.us-east-1.amazonaws.com/";

    let email_body = r#"From: "Your Name" <your-email@example.com>
To: james@company.com
Subject: Hi mr james
MIME-Version: 1.0
Content-Type: text/plain; charset=UTF-8

I am sure we will meet again
"#;

    let request_body = format!(
        "Action=SendRawEmail&Source=your-email@example.com&Destinations.member.1=james@company.com&RawMessage.Data={}",
        general_purpose::STANDARD.encode(email_body)
    );

    let date = Utc::now().format("%a, %d %b %Y %H:%M:%S GMT").to_string();
    let signature = create_aws_signature(&date, secret_key, region);

    let client = Client::new();
    let response = client
        .post(&ses_endpoint)
        .header("X-Amz-Date", &date)
        .header("Authorization", signature)
        .header("Content-Type", "application/x-www-form-urlencoded")
        .body(request_body)
        .send();

    match response {
        Ok(resp) => println!("Email sent successfully: {:?}", resp.text()),
        Err(err) => eprintln!("Failed to send email: {}", err),
    }
}

fn create_aws_signature(date: &str, secret_key: &str, region: &str) -> String {
    let string_to_sign = format!("AWS4-HMAC-SHA256\n{}\n{}/ses/aws4_request", date, region);
    let mut mac = HmacSha256::new_from_slice(secret_key.as_bytes()).expect("HMAC can take key of any size");
    mac.update(string_to_sign.as_bytes());
    let signature = hex::encode(mac.finalize().into_bytes());
    format!("AWS4-HMAC-SHA256 Credential=/{}", signature)
}
