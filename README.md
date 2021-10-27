# Kuna.Net
c# api wrapper for @kunadevelopers https://kuna.io crypto exchange

[![publish to nuget](https://github.com/ridicoulous/Kuna.Net/actions/workflows/publish.yml/badge.svg)](https://github.com/ridicoulous/Kuna.Net/actions/workflows/publish.yml)


Kuna.Net is a .Net wrapper for the [Kuna.io exchnage API](https://kuna.io/) as described at [docs](https://docs.kuna.io/docs). It includes all features the API provides using clear and readable C# objects including 
* Reading market info
* Placing and managing orders
* Reading balances and funds

Additionally it adds some convenience features like:
* Configurable rate limiting
* Autmatic logging
## Installation
![Nuget version](https://img.shields.io/nuget/v/Kuna.Net.svg) ![Nuget downloads](https://img.shields.io/nuget/dt/Kuna.Net.svg)

Available on [NuGet](https://www.nuget.org/packages/Kuna.Net/):
```
PM> Install-Package Kuna.Net
```
To get started with Kuna.Net first you will need to get the library itself. The easiest way to do this is to install the package into your project using [NuGet](https://www.nuget.org/packages/Kuna.Net/).
see [integration tests for minimal examples](https://github.com/ridicoulous/Kuna.Net/blob/master/Kuna.Net.Tests/IntegrationTests.cs)

## CryptoExchange.Net
Implementation is build upon the CryptoExchange.Net library, make sure to also check out the documentation on that: [docs](https://github.com/JKorf/CryptoExchange.Net)

Other CryptoExchange.Net implementations:
<table>
<tr>
<td><a href="https://github.com/JKorf/Bitfinex.Net"><img src="https://github.com/JKorf/Bitfinex.Net/blob/master/Bitfinex.Net/Icon/icon.png?raw=true"></a>
<br />
<a href="https://github.com/JKorf/Bitfinex.Net">Bitfinex</a>
</td>
<td><a href="https://github.com/JKorf/Bittrex.Net"><img src="https://github.com/JKorf/Bittrex.Net/blob/master/Bittrex.Net/Icon/icon.png?raw=true"></a>
<br />
<a href="https://github.com/JKorf/Bittrex.Net">Bittrex</a>
</td>
<td><a href="https://github.com/JKorf/Binance.Net"><img src="https://github.com/JKorf/Binance.Net/blob/master/Binance.Net/Icon/icon.png?raw=true"></a>
<br />
<a href="https://github.com/JKorf/Binance.Net">Binance</a>
</td>
<td><a href="https://github.com/JKorf/CoinEx.Net"><img src="https://github.com/JKorf/CoinEx.Net/blob/master/CoinEx.Net/Icon/icon.png?raw=true"></a>
<br />
<a href="https://github.com/JKorf/CoinEx.Net">CoinEx</a>
</td>
<td><a href="https://github.com/JKorf/Huobi.Net"><img src="https://github.com/JKorf/Huobi.Net/blob/master/Huobi.Net/Icon/icon.png?raw=true"></a>
<br />
<a href="https://github.com/JKorf/Huobi.Net">Huobi</a>
</td>
<td><a href="https://github.com/JKorf/Kucoin.Net"><img src="https://github.com/JKorf/Kucoin.Net/blob/master/Kucoin.Net/Icon/icon.png?raw=true"></a>
<br />
<a href="https://github.com/JKorf/Kucoin.Net">Kucoin</a>
</td>
<td><a href="https://github.com/JKorf/Kraken.Net"><img src="https://github.com/JKorf/Kraken.Net/blob/master/Kraken.Net/Icon/icon.png?raw=true"></a>
<br />
<a href="https://github.com/JKorf/Kraken.Net">Kraken</a>
</td>
<td><a href="https://github.com/Zaliro/Switcheo.Net"><img src="https://github.com/Zaliro/Switcheo.Net/blob/master/Resources/switcheo-coin.png?raw=true"></a>
<br />
<a href="https://github.com/Zaliro/Switcheo.Net">Switcheo</a>
</td>
<td><a href="https://github.com/ridicoulous/LiquidQuoine.Net"><img src="https://github.com/ridicoulous/LiquidQuoine.Net/blob/master/Resources/icon.png?raw=true"></a>
<br />
<a href="https://github.com/ridicoulous/LiquidQuoine.Net">Liquid</a>
</td>
<td><a href="https://github.com/burakoner/OKEx.Net"><img src="https://raw.githubusercontent.com/burakoner/OKEx.Net/master/Okex.Net/Icon/icon.png"></a>
<br />
<a href="https://github.com/burakoner/OKEx.Net">OKEx</a>
</td>
<td><a href="https://github.com/d-ugarov/Exante.Net"><img src="https://github.com/d-ugarov/Exante.Net/blob/master/Exante.Net/Icon/icon.png?raw=true"></a>
<br />
<a href="https://github.com/d-ugarov/Exante.Net">Exante</a>
</td>
</tr>
</table>

## Changelog
* 10/27/2021 
  * turn off pro account on error
* 9/29/2021 
    * major base library update
* 05/14/2021 
    * base library update
* 03/25/2021 
    * new api version 3 implemented

## Donations
Donations are greatly appreciated and a motivation to keep improving.

**Btc**:  14nuXrFEKTrvyhHWYW7RgRt4zVxBfwff5V  
**Eth**:  0x6Fea7665684584884124C1867d7eC31B56C43373 
