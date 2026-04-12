class EngineSoundGenerator extends AudioWorkletProcessor {
	constructor(opts) {
		super();

		this.sampleRate = sampleRate;

		this.rpm = 3000;
		this.throttle = 1;

		this.freqScale = opts.processorOptions.freqScale;
		this.baseDuty = opts.processorOptions.baseDuty;
		this.throttleDuty = opts.processorOptions.throttleDuty;
		this.waves = opts.processorOptions.waves;
		this.phase = new Array(this.waves.length).fill(0);

		this.port.onmessage = (e) => {
			if (e.data.rpm != undefined) {
				this.rpm = e.data.rpm;
			}
			if (e.data.throttle != undefined) {
				this.throttle = e.data.throttle;
			}
		}
	}

	sample() {
		let baseFreq = this.rpm * this.freqScale;
		let sub = 0;
		let totalVolume = 0;

		for (let j = 0; j < this.waves.length; j++) {
			let wave = this.waves[j];

			this.phase[j] += (baseFreq * wave.overtone * (2 * Math.PI)) / this.sampleRate;
			if (this.phase[j] > Math.PI * 2) this.phase[j] -= Math.PI * 2;

			totalVolume += wave.volume;

			let dutyExp = wave.baseDuty + this.baseDuty * wave.dutyScale +
				(this.throttle > 0.1 ? 1 : 0) * this.throttleDuty * wave.throttleDutyScale;

			let p = Math.sin(this.phase[j] + wave.offset);
			sub += wave.volume * Math.pow(Math.abs(p), Math.floor(dutyExp)) * Math.sign(p);
		}

		return totalVolume > 0 ? (sub / totalVolume) : 0;
	}

	process(inputs, outputs, parameters) {
		const output = outputs[0];

		for (let i = 0; i < output[0].length; i++) {
			const v = this.sample();
			for (let j = 0; j < output.length; j++) output[j][i] = v;
		}

		return true;
	}
}

registerProcessor("EngineSoundGenerator", EngineSoundGenerator);

