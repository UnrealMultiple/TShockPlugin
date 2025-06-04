# QRCoder QR code generator

- Author: Jonesn、熙恩、Radix.
- Source: TShock China official QQ group 
- A QR code pattern wall generated on the map through instructions.
- Supports custom position, size and alignment, and the recommended size 5 is available.
- The QR code is made of crystal diamond wall painted with white paint, shadow paint and night paint, and for the sake of beauty, there is a circle of echo walls on the edge.

## Instruction

| Command                     |     Permissions      |              Description               |
|------------------------|:-----------:|:-----------------------------:|
| /qr <content> [size(if not specified, it is adaptive)]       | qr.add      |           Generate QR code            |
| /qrpos <tl\|tr\|bl\|br>  | qr.add | Set the position alignment method of the QR code (upper left corner, upper right corner, lower left corner, lower right corner) |
| /qrconf <key> <value> | qr.add | Set QR code configuration. Keys: BaseWall/bw/底墙（int）、BaseColor/bc/底墙颜色（int）、CodeWall/cw/码墙（int）、CodeColor/cc/码墙颜色（int）、IlluminantCoating/ic/夜明涂料（true/false）、QRErrorCorrectionLevel/qrlevel/纠错等级（int） |

## Configuration

```json5
{
  "BaseWall": 155,
  "BaseColor": 26,
  "CodeWall": 155,
  "CodeColor": 29,
  "IlluminantCoating": true
}
```

## Feedback
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
