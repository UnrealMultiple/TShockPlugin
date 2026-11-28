# Noagent Anti-proxy plugin

- author: Jonesn，肝帝熙恩
- source: TShock Chinese official group
- Calls `https://blackbox.ipinfo.app/lookup/` by default, and the actual measurement effect is OK; set `Provider` to use [VPNDetection](https://vpndetection.io) instead, which blocks VPN and Tor with no key ([terms](https://vpndetection.io/terms#no-key), [privacy](https://vpndetection.io/privacy))

## Instruction

```
none
```

## Configuration
> Configuration file location: tshock/Noagent.en-US.json
```json5
{
  "Provider": "blackbox" // blackbox or vpndetection
}
```

## Feedback
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
