-- phpMyAdmin SQL Dump
-- version 5.2.3
-- https://www.phpmyadmin.net/
--
-- Gép: 127.0.0.1:3306
-- Létrehozás ideje: 2026. Okt 22. 06:10
-- Kiszolgáló verziója: 8.4.7
-- PHP verzió: 8.3.28

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Adatbázis: `receptek`
--

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `hozzavalo`
--

DROP TABLE IF EXISTS `hozzavalo`;
CREATE TABLE IF NOT EXISTS `hozzavalo` (
  `id` int NOT NULL AUTO_INCREMENT,
  `ingredientName` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `quantity` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `description` text COLLATE utf8mb4_unicode_ci,
  `addedTime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updateTime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `szakacsId` int NOT NULL,
  PRIMARY KEY (`id`),
  KEY `szakacsId` (`szakacsId`)
) ENGINE=MyISAM AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- A tábla adatainak kiíratása `hozzavalo`
--

INSERT INTO `hozzavalo` (`id`, `ingredientName`, `quantity`, `description`, `addedTime`, `updateTime`, `szakacsId`) VALUES
(1, 'Liszt', '500 g', 'Finomliszt, sütéshez.', '2025-05-05 09:00:00', '2025-05-05 09:00:00', 1),
(2, 'Paradicsom', '1 kg', 'Érett, házi kertből.', '2025-05-16 10:30:00', '2025-05-16 10:30:00', 2),
(3, 'Fokhagyma', '3 gerezd', 'Frissen pucolva.', '2025-05-27 11:15:00', '2025-05-27 11:15:00', 3),
(4, 'Vaj', '250 g', 'Sótlan, hűtőből.', '2025-06-07 08:20:00', '2025-06-07 08:20:00', 5),
(5, 'Bazsalikom', '1 csokor', 'Friss, tésztaszószhoz.', '2025-06-18 12:40:00', '2025-06-18 12:40:00', 6);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `szakacs`
--

DROP TABLE IF EXISTS `szakacs`;
CREATE TABLE IF NOT EXISTS `szakacs` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `email` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `age` int NOT NULL,
  `password` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `registrationTime` datetime NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- A tábla adatainak kiíratása `szakacs`
--

INSERT INTO `szakacs` (`id`, `name`, `email`, `age`, `password`, `registrationTime`) VALUES
(1, 'Bíró Emma', 'biro.emma@example.com', 29, 'receptes1', '2025-01-09 08:45:00'),
(2, 'Székely Zalán', 'szekely.zalan@example.com', 34, 'konyha2025', '2025-02-02 10:20:00'),
(3, 'Antal Flóra', 'antal.flora@example.com', 27, 'fuszeres99', '2025-02-21 09:00:00'),
(4, 'Pintér Marcell', 'pinter.marcell@example.com', 31, 'sutes321', '2025-03-11 14:30:00'),
(5, 'Nagy Alíz', 'nagy.aliz@example.com', 25, 'recept2025', '2025-03-30 11:15:00'),
(6, 'Váradi Botond', 'varadi.botond@example.com', 36, 'grillmester', '2025-04-17 15:50:00');
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
